# 🚀 Corporate Task Management System (Intranet App)

This project is a modern task tracking and management system designed for corporate intranet environments. Built with **ASP.NET Core MVC**, it moves beyond traditional list views by implementing a **Kanban Board** methodology and a robust **Relational Database Management System (RDBMS)** architecture.

## 🌍 Live Demo

🚀 **Check out the live application here:** 👉 [**https://ik-takip-sistemi.onrender.com/**](https://ik-takip-sistemi.onrender.com/)

*(Note: Since the application is hosted on **Render.com's free tier**, the server spins down after periods of inactivity. The first request might take **20-30 seconds** to wake up. Please be patient!)*

---

## 🛠️ Tech Stack & Infrastructure

This project leverages modern cloud technologies for deployment and data management:

* **Framework:** .NET 8 (ASP.NET Core MVC)
* **Language:** C#
* **Database:** PostgreSQL (**Neon.tech Serverless**) ☁️
* **Hosting/Deployment:** **Render.com** (CI/CD Pipeline) 🚀
* **ORM:** Entity Framework Core (Code-First Approach)
* **Frontend:** Bootstrap 5, HTML5, CSS3, Razor Views
* **Authentication:** ASP.NET Core Identity (Customized)

## ✨ Key Features

### 1. Relational Database Architecture 🏗️
Unlike simple CRUD apps, this project ensures data integrity through proper normalization and Foreign Key relationships.
* **One-to-Many Relationships:** Tasks are linked to users via `Guid` (Foreign Keys), ensuring that user profile updates (e.g., name changes) are instantly reflected across all historical tasks.
* **Data Integrity:** Prevents data redundancy and orphan records.

### 2. Dynamic Kanban Board 📋
A visual project management tool that categorizes tasks into three dynamic states:
* **To Do**
* **In Progress**
* **Done**

Users can visualize workflow and bottlenecks at a glance rather than reading static tables.

### 3. Customized Identity System 👤
The standard `IdentityUser` class has been extended to `ApplicationUser` to support custom profile fields (e.g., `FullName`), providing a more personalized user experience instead of relying solely on email addresses.

### 4. Real-Time Dashboard 📊
The home page features a live dashboard that aggregates data using LINQ queries:
* **Personal Workload:** Shows the count of active tasks assigned specifically to the logged-in user.
* **Global Stats:** Displays total tasks, pending work, and completed projects.

## 📷 Screenshots
*(Screenshots will be changed with each update.)*

<img width="1920" height="1022" alt="ik-takip1" src="https://github.com/user-attachments/assets/b476636c-ff38-4e12-b381-4cf12c8064a8" />

---
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/312c9cca-0536-4b9a-97d4-7f722955f210" />

---
<img width="1920" height="1029" alt="image" src="https://github.com/user-attachments/assets/b71c51b7-5409-4f50-b0f2-24c140cf58eb" />

---

## ⚙️ Installation & Setup (Local)

1.  **Clone the repository**
    ```bash
    git clone https://github.com/OguzAnilAtes/IK_Intranet_App.git
    ```

2.  **Configure Database**
    Update the `ConnectionStrings` in `appsettings.json` with your PostgreSQL credentials.

3.  **Apply Migrations**
    Open the Package Manager Console (PMC) and run:
    ```powershell
    Update-Database
    ```

4.  **Run the Application**
    Build and run the project via Visual Studio or CLI:
    ```bash
    dotnet run
    ```

## 🗺️ Roadmap

* [x] Kanban Board Implementation
* [x] Relational Database Refactoring (User-Task Association)
* [x] Dynamic Dashboard
* [x] Cloud Deployment (Render & Neon)
* [ ] Role-Based Authorization (Admin/User)
* [ ] Email Notifications

---
*Developed by [Oğuz Anıl ATEŞ](https://www.linkedin.com/in/oguzanilates)*
