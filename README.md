# 🎓 Training Center Management Platform

## 🌟 Introduction

Educational centers often struggle with managing course offerings, monitoring student enrollments, and tracking academic progress. Similarly, students may face difficulties finding suitable courses or managing their registrations effectively.

This project addresses those challenges by developing a centralized web platform that enables **efficient management of training courses**, **student registration**, and **real-time course tracking**. It provides tools for both **administrators** and **students** to streamline educational management and improve the overall learning experience.

In addition to solving practical problems, this project is also a hands-on opportunity for our development team to apply key software engineering principles, such as MVC architecture, Entity Framework usage, and database-driven application development.

---

## ✨ Core Features

Our platform supports **two main user roles** — **Students** and **Administrators** — each with tailored functionalities:

---

### 👨‍🏫 For Administrators

Administrators have full access to manage the entire system, including course offerings and student accounts.

- **Course Management**  
  Create, update, delete, and view details of training courses.  
  Includes fields such as:  
  - Course ID, Name, Instructor  
  - Start Date, Tuition Fee, Maximum Capacity  

- **Student Management**  
  Manage student records including:  
  - Student ID, Full Name, Date of Birth, Email, Phone Number, Username, Password  

- **Enrollment Monitoring**  
  View and manage student enrollment per course.  
  Enforce constraints like max student capacity and enrollment deadlines.

---

### 🎓 For Students

Students can interact with the system through a streamlined interface designed to help them discover and register for courses.

- **Account Registration & Login**  
  Easily sign up and securely log in to the platform.

- **Course Browsing & Registration**  
  View available courses and register based on personal preferences.  
  The system will check for course availability before confirming registration.

- **Enrollment Cancellation**  
  Cancel registration if needed (prior to course start date).

---
## 🛠 Technology Stack

- **Backend:** ASP.NET MVC (C#)
- **Frontend:** HTML, CSS, Bootstrap
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Architecture:** Model-View-Controller (MVC)

