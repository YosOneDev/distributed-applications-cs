# Sports Events Management System

**Student Faculty Number:** 2301261057  
**Project Name:** Sports Events Management System  

## Project Description
This web application allows management of sports events, participants and registrations.  
It includes full CRUD operations, user authentication, role-based access (Guest, User, Admin),  
sorting, searching, pagination, and an additional feature for uploading and approving event images.

The system consists of:
- ASP.NET Core MVC application  
- MS SQL Server database  
- Entity Framework Core  
- Identity (Users + Roles)

## Installation Instructions

1. **Clone or download the project**  
   Place it in:  
   `course-work/implementations/2301261057/`

2. **Open the solution**  
   Open `SportsEvents.Web.sln` in **Visual Studio 2022** or newer.

3. **Configure the database**  
   In `appsettings.json`, confirm the connection string to SQL Server or LocalDB.

4. **Apply EF Core migrations**  
   Open **Package Manager Console** and run:
   ```
   update-database
   ```

5. **Run the application**  
   Start the project using **IIS Express** or **Kestrel**.

## Default Admin Account
The system automatically creates an administrator:

```
Email: admin@sportsevents.com
Password: Admin123!
```

This user has full permissions (Create, Edit, Delete, Approve Images, Manage Roles).

---

## Additional Notes
The project fully follows the requirements for:
- CRUD operations for all tables  
- 3-level access security  
- Searching, sorting, pagination  
- Admin panel for role management  
- Bonus feature: image upload + approval  
