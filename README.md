# StanaGO

**StanaGO** is a web platform dedicated to the digitalization of traditional Romanian pastoral commerce. The application bridges the gap between local producers (shepherds) and tourists, facilitating the trade of organic traditional products while enhancing safety through real-time hazard alerts.

##  Key Features

* **Interactive Mapping:** Visualization of sheepfolds and their products using **Leaflet.js** and **OpenStreetMap**.
* **Real-Time Alert System:** Reporting and displaying hazards (e.g., bears, wolves) in real-time, featuring distance-based filtering to keep tourists safe.
* **Integrated Chat:** A private messaging system for direct communication between consumers and producers.
* **Product Management:** A dedicated dashboard for shepherds to manage their inventory, pricing, and availability.
* **Role-Based Access Control (RBAC):** Secure, differentiated access levels for tourists, producers, and administrators.
* **Responsive Design:** A mobile-first approach ensuring the platform is accessible from any device, even in remote areas.

---

##  Tech Stack

The project is built using a modern enterprise-grade stack:

* **Framework:** ASP.NET Core MVC
* **Language:** C#
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core
* **Frontend:** Razor Pages, Bootstrap 5, JavaScript
* **Maps API:** Leaflet.js



---

##  Installation & Setup

Follow these steps to get the project running locally:

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/Ciorbingus/StanaGO.git](https://github.com/Ciorbingus/StanaGO.git)
    ```

2.  **Database Configuration:**
    Open `appsettings.json` and update the `ConnectionStrings` section with your local SQL Server instance details:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=StanaGO;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
    ```

3.  **Apply Migrations:**
    Create the database schema by running:
    ```bash
    dotnet ef database update
    ```

4.  **Run the Application:**
    ```bash
    dotnet run
    ```

---

## The Team

* **Popescu Florian**
* **Todi Tinu-Constantin**

---
*Digitalizing traditions with StanaGO.*
