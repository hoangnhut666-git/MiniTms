Here is the English version of the technical specification document, formatted in Markdown as requested.

---

# TECHNICAL SPECIFICATION: MINI-TMS (TRANSPORTATION MANAGEMENT SYSTEM)
## TECHNOLOGY UPDATE VERSION (.NET 10, EF CORE, SQL SERVER, XUNIT, PLAYWRIGHT)

---

## 1. SYSTEM OVERVIEW

### 1.1. General Introduction
The **Mini-TMS** system is a scaled-down version of a full Transportation Management System. It is designed to optimize the delivery route planning process (Vehicle Routing Problem - VRP). The system accepts a list of orders, fleet information, and rate card configurations to automatically calculate an optimal order consolidation and route selection plan that satisfies constraints regarding vehicle capacity, delivery time windows, and geographical areas.

### 1.2. Document Objectives
This document provides a detailed specification of the architecture, data model, core business logic, and testing strategy for the updated Mini-TMS system following its technology stack migration. The system has been completely restructured using modern technologies, aiming for flexible data management, robust logic, and a comprehensive automated testing process.

---

## 2. TECHNOLOGY STACK

The system is developed based on a layered architecture (API, Core, Data) utilizing the following enterprise-standard technologies:

| Layer | Technology | Role / Responsibility |
| :--- | :--- | :--- |
| **API** | ASP.NET Core Web API (.NET 10) | Provides RESTful API Endpoints via Controllers to receive requests, orchestrate services, and return JSON results to the Client. Integrates Swagger/OpenAPI for public API documentation. |
| **Data Access** | **Entity Framework Core** | Acts as the Object-Relational Mapper (ORM), managing connections, mapping Entities, handling complex relationships via Navigation Properties, and executing queries using LINQ. |
| **Database (DB)** | Microsoft SQL Server 2022 | Relational database storing master data (Customers, Vehicles, Rate Cards) and operational data (Orders, persisted Trip plans). |
| **Core Logic** | Google.OrTools (NuGet Package) | An advanced optimization algorithm library, responsible for solving multi-constraint Vehicle Routing Problems (VRP) in real-time. |
| **Testing** | **xUnit + Playwright** | **xUnit**: Builds Unit Tests for isolated core logic (Region Split, Cost Calculator, Capacity Constraints). <br>**Playwright**: Performs Automation API Testing and End-to-End (E2E) Testing, simulating real user API call flows. |

---

## 3. DATA MODEL & ENTITIES (EF CORE)

The migration from Dapper to EF Core allows the system to define entities with tightly coupled relationships using Navigation Properties. The detailed structure of each Entity class is as follows:

### 3.1. Customer
Stores detailed information about delivery points/partner recipients.
*   `CustomerId` (int, PK, Identity)
*   `Name` (string, Required, MaxLength: 250): Customer or store name.
*   `District` (string, Required, MaxLength: 100): District used for rate calculation.
*   `Latitude` (double, Required): Latitude for distance and region-splitting algorithms.
*   `Longitude` (double, Required): Longitude for distance and region-splitting algorithms.
*   `IsVip` (bool, Default: false): Flag marking a priority customer.
*   **Navigation Properties:** `ICollection<Order> Orders` (1-to-N relationship with Orders).

### 3.2. Order
Information about orders needing delivery for the day.
*   `OrderId` (int, PK, Identity)
*   `CustomerId` (int, FK): Customer identifier receiving the order.
*   `WeightKg` (double, Required): Weight of the order (unit: kg).
*   `OrderDate` (DateOnly, Required): Date the order is due for delivery.
*   `DeliveryStart` (TimeOnly, Required): Time window start for delivery.
*   `DeliveryEnd` (TimeOnly, Required): Time window end for delivery.
*   `Status` (string, MaxLength: 50, Default: 'NEW'): Order status (NEW, PLANNED, REJECTED, REJECTED_TIMEWINDOW).
*   **Navigation Properties:**
    *   `Customer Customer`: Back-reference to customer details.
    *   `TripOrder? TripOrder`: Reference to routing assignment (if assigned to a trip).

### 3.3. Vehicle
The fleet of vehicles available for transportation.
*   `VehicleId` (int, PK, Identity)
*   `Plate` (string, Required, MaxLength: 20): License plate.
*   `VendorCode` (string, Required, MaxLength: 50): Carrier code (e.g., V1, V2).
*   `VehicleType` (string, Required, MaxLength: 20): Payload class (e.g., 1.5T, 2.5T, 5T).
*   `CapacityKg` (double, Required): Maximum payload capacity (unit: kg).
*   `IsActive` (bool, Default: true): Vehicle's operational status (inactive vehicles are excluded from the solver).
*   **Navigation Properties:** `ICollection<Trip> Trips` (1-to-N relationship with Trips).

### 3.4. RateCard
Carrier rate sheet for calculating transportation costs based on carrier and destination district.
*   `RateCardId` (int, PK, Identity)
*   `VendorCode` (string, Required, MaxLength: 50): Carrier code applying this rate.
*   `ToDistrict` (string, Required, MaxLength: 100): Destination district.
*   `VehicleType` (string, Required, MaxLength: 20): Vehicle type this rate applies to.
*   `BaseCost` (decimal, Required): Base cost for the first stop.
*   `DropFee` (decimal, Required): Surcharge applied for each additional stop (from the second stop onward).
*   `IsActive` (bool, Default: true): Indicates if the rate entry is active.

### 3.5. Trip (Planned Trip)
The high-level output result from the optimization solver.
*   `TripId` (int, PK, Identity)
*   `PlanCode` (string, Required, MaxLength: 100): Execution plan code (e.g., `COST-20260523-143000`).
*   `VehicleId` (int, Required, FK): Vehicle assigned to this trip.
*   `TotalKm` (double, Required): Total actual travel distance following the circular route (Depot -> Stops -> Depot).
*   `TotalCost` (decimal, Required): Total actual cost for the trip after rate calculation and fallback.
*   `Strategy` (string, Required, MaxLength: 20): Optimization strategy used (COST, TRIPS, DISTANCE).
*   **Navigation Properties:**
    *   `Vehicle Vehicle`: Detailed information on the executing vehicle.
    *   `ICollection<TripOrder> TripOrders`: List of detailed stops in sequence order.

### 3.6. TripOrder (Stop/Order Detail within a Trip)
A junction table structuring the stop sequence (Route Stops) for each specific trip.
*   `TripOrderId` (int, PK, Identity)
*   `TripId` (int, Required, FK): Which trip this belongs to.
*   `OrderId` (int, Required, FK): Which order is delivered at this stop.
*   `StopIndex` (int, Required): Delivery sequence number (1, 2, 3...).
*   `EtaTime` (TimeOnly, Required): Estimated Time of Arrival at the delivery point.
*   **Navigation Properties:**
    *   `Trip Trip`: Reference back to the parent Trip.
    *   `Order Order`: Direct reference to the delivered order's details.

---

## 4. CORE BUSINESS PROCESS & MATHEMATICAL LOGIC

The system solves the transportation problem through a sequential process combining real-world logistics business constraints:

```
[Accept Order List]
       │
       ▼
[Geographical Region Split] ---> Separate orders for North/South Red River area
       │
       ▼
[Parallel Multi-Strategy Processing] ---> Run COST, TRIPS, DISTANCE (Task.WhenAll)
       │
       ▼
[Google.OrTools Solver] ----------> Calculate VRP satisfying Capacity, TimeWindow, MaxStops
       │
       ▼
[Rate Calculation & Vehicle Fallback] ----> Lookup rates, auto-upgrade vehicle type if rate missing
       │
       ▼
[EF Core DB Transaction] ----> Persist to Trips and TripOrders tables atomically
```

### 4.1. Latitude-based Region Split
To optimize OrTools solver processing time and reflect operational reality (e.g., limiting bridge crossings causing traffic jams), the system applies an order-splitting mechanism:
*   **Fixed Boundary Latitude:** `Latitude = 21.03` (Simulating the Red River geographical boundary in Hanoi).
*   **Split Rules:**
    *   If `Order.Latitude >= 21.03` → Assign order to **North region**.
    *   If `Order.Latitude < 21.03` → Assign order to **South region**.
*   Each region's set of orders is then passed to the solver for completely independent processing.

### 4.2. Multi-Constraint VRP Solver (Google.OrTools)
The routing optimization algorithm enforces the following hard constraints:
1.  **Central Depot Coordinates:** Fixed at Cau Giay (`Latitude = 21.0285`, `Longitude = 105.7822`).
2.  **Distance Matrix Callback:** Distance between Nodes (Depot and delivery points) is calculated using the Haversine formula (unit: meters).
3.  **Capacity Dimension:** Total order weight on a single vehicle must be less than or equal to that vehicle's capacity ($\sum WeightKg \le Vehicle.CapacityKg$). If a single order's weight exceeds the largest vehicle's capacity, the solver places that order into the `RejectedOrderIds` list.
4.  **Time Window Dimension:**
    *   Assumed intra-city travel speed: $40 km/h$. Travel time (minutes) = $\frac{Distance(km)}{40} \times 60$.
    *   Service time per stop: Fixed at $20$ minutes per order.
    *   Vehicle departure from depot: $06:00 AM$ (360 minutes since midnight).
    *   Actual arrival time must fall within the interval: $[DeliveryStart, DeliveryEnd]$. If infeasible due to distance or time conflicts, the order is dropped/rejected from the route to maintain feasibility.
5.  **Max Stops Dimension:** The maximum number of orders assigned to a single trip must not exceed the input parameter `MaxStopsPerTrip` (Default = 3).

### 4.3. Cost Calculation Formula & Large Vehicle Fallback Mechanism
After the solver proposes a consolidation list, the system calculates the actual financial cost:
*   **Basic Cost Formula:**
    $$TotalCost = BaseCost + (StopCount - 1) \times DropFee$$
    *(Where $StopCount$ is the actual number of orders on that trip).*
*   **Vehicle Fallback Mechanism (Auto Upgrading):**
    *   The defined vehicle size order from smallest to largest: `1.5T` → `2.5T` → `5T`.
    *   When a trip uses a `1.5T` vehicle, the system searches for a matching `RateCard` entry using the same `VendorCode` and `ToDistrict`.
    *   If **no exact rate exists** for `1.5T` in that district, the system automatically searches for the rate of the next larger vehicle type, `2.5T` (same carrier, same district). If still missing, it continues searching for the `5T` rate.
    *   The returned `UsedVehicleType` value reflects the actual vehicle type used for cost calculation. This ensures the system never encounters a rate calculation error due to missing detailed configuration for smaller vehicles.

### 4.4. Parallel Asynchronous Multi-Strategy Optimization
The system provides a plan comparison mechanism using 3 strategies, executed simultaneously:
*   **COST:** Optimizes total cash cost paid to the carrier (prioritizes consolidating orders onto vehicles with the cheapest rates).
*   **TRIPS:** Optimizes the total number of trips dispatched (encourages max stops per vehicle to minimize dispatch count, applies a $500đ penalty cost per dispatched trip).
*   **DISTANCE:** Optimizes total travel distance ($km$) for the entire fleet (does not consider cash cost).
The system uses asynchronous multi-threading (`Task.WhenAll`) to simultaneously trigger the solver for all 3 strategies. This maximizes CPU hardware utilization, ensuring optimal API response time.

---

## 5. API ENDPOINTS DESIGN (ASP.NET CORE)

The system provides the following REST API endpoints for transportation data management:

### 5.1. GET /api/orders
Retrieves the list of all orders scheduled for processing on a specific date.
*   **Query Parameters:** `date` (Type: `DateOnly`, defaults to today's date).
*   **Response (OK - 200):** Returns a JSON array containing order information along with corresponding Customer data (loaded via `.Include(o => o.Customer)` in EF Core).

### 5.2. GET /api/vehicles
Retrieves the list of available transportation vehicles.
*   **Response (OK - 200):** List of vehicles where `IsActive == true`, sorted ascending by `CapacityKg`.

### 5.3. POST /api/optimize
Executes a single optimization planning run for a specific date based on a specified strategy.
*   **Request Body (JSON):**
    ```json
    {
      "date": "2026-05-23",
      "strategy": "COST",
      "maxStopsPerTrip": 3
    }
    ```
*   **Internal Processing:** Triggers `OrderRepository`, `VehicleRepository` via EF Core, calls `VrpSolver.Solve()`, performs rate calculation, then opens a **Database Transaction** (`DbContext.Database.BeginTransactionAsync()`) to atomically save data to the `Trips` and `TripOrders` tables.
*   **Response (OK - 200):** Returns the generated `planCode`, a list of `trips` with detailed stop information (including specific `eta` times), and a list of `rejectedOrderIds`.

### 5.4. POST /api/optimize/multi
Executes a simulation running all 3 strategies (COST, TRIPS, DISTANCE) simultaneously to allow logistics operators to choose the best plan.
*   **Request Body (JSON):**
    ```json
    {
      "date": "2026-05-23",
      "maxStopsPerTrip": 3
    }
    ```
*   **Response (OK - 200):** A JSON array containing 3 elements, corresponding to the analysis results for each strategy. Displays `totalCost`, `totalTrips`, and `totalKm` for each scenario for comparison.

---

## 6. COMPREHENSIVE TESTING STRATEGY (XUNIT & PLAYWRIGHT)

The system employs a layered testing model to ensure absolute stability, from core business logic processing to integrated end-to-end API data flows.

### 6.1. Unit Testing with xUnit (Backend Core Logic)
Unit tests are set up independently, eliminating physical I/O dependencies by using Mock/Stub data or employing the **EF Core InMemory Database Provider** to replace the actual SQL Server connection in the test environment.

Mandatory Unit Test scenarios to cover:
*   **Scenario 1 - Region Split Test:** Load a list of orders with coordinates from both North/South Red River zones (e.g., Tay Ho latitude 21.07, Ha Dong latitude 20.97). The `VrpSolver.SplitByRegion` function must accurately classify the order count per region without losing any orders. Test the edge case where an order lies exactly at the boundary latitude `21.03`.
*   **Scenario 2 - Vehicle Fallback Test:** Create a mock rate card table (using `InMemoryDb`) where Long Bien district **only** has a rate configuration for `2.5T` vehicles and **no** configuration for `1.5T`. When executing the `CostCalculator.Calculate` function with input parameters for a `1.5T` vehicle going to Long Bien district, the result must prove the system automatically uses the `2.5T` rate (`UsedVehicleType == "2.5T"`).
*   **Scenario 3 - Capacity Constraint Test:** Initialize an exceptionally heavy order ($900 kg$). When passed to the optimization solver, verify that the solver correctly bypasses smaller vehicles like the `1.5T` (low capacity) to accurately assign this order to a large `5T` truck.
*   **Scenario 4 - Max Stops Test:** Load 5 orders into the same vehicle's zone. Set the `maxStopsPerTrip = 2`. Verify the output result splits these into at least 3 independent trips, ensuring no trip contains more than 2 delivery points.

### 6.2. End-to-End (E2E) & API Integration Testing with Playwright
Playwright is integrated into the xUnit runner to perform automated black-box testing directly against the running application.

Playwright E2E Test Execution Procedure:
1.  **Start Virtual Host:** The test suite starts a background instance of the API Web Server (bound to a random Localhost port).
2.  **Initialize Background Client:** Playwright uses `APIRequestContext` to send an actual HTTP `POST` request to the `/api/optimize/multi` endpoint with a test JSON payload.
3.  **Validation Assertions:**
    *   Verify the HTTP response status code is `200 OK`.
    *   Parse the returned JSON string to ensure it has the array structure for all 3 strategies (`COST`, `TRIPS`, `DISTANCE`).
    *   Check financial logic: The `totalCost` for the `COST` strategy must be less than or equal to the `totalCost` for the `DISTANCE` strategy.
    *   Check data accuracy: All `tripId`s returned in the JSON must actually exist in the database (using a separate DbContext to cross-check the Test SQL Server), ensuring the data writing flow and Transaction succeeded with no orphaned data.
4.  **Performance Measurement:** Check the total response time of the parallel multi-strategy API, ensuring it does not exceed a specified threshold (e.g., < 8 seconds). This demonstrates that the asynchronous multi-threading technique meets high-performance requirements.