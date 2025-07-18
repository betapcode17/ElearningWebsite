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

  
## 📽️ Program Demo

### 1. 👤 Guest Interface

#### 1.1: Landing Page
Visitors are welcomed with a homepage showcasing featured courses and an overview of the platform.

#### 1.2: Course List
Non-logged-in users can browse available courses including names, instructors, categories, and start dates.

#### 1.3: Course Details
Each course includes detailed information like description, tuition, available slots, and schedule.

#### 1.4: Instructor Introduction
Guests can view profiles of instructors before deciding to register.

#### 1.5: Login & Registration Page
Users can register either as a student or instructor. The login page includes validation and role-based redirects.

---

### 2. 👨‍🎓 Student Interface

#### 2.1: Student Dashboard
Students see a personalized dashboard listing enrolled courses, upcoming classes, and progress summaries.

#### 2.2: Course Enrollment
Students can enroll in courses with real-time slot validation and receive confirmation.

#### 2.3: My Courses
Displays a student's enrolled courses, including details like start date, instructor, and status.

#### 2.4: Cancel Enrollment
Students can cancel course registrations before the start date.

#### 2.5: Update Profile
Students can update personal info including name, email, and password.

---

### 3. 👨‍🏫 Instructor Interface

#### 3.1: Instructor Dashboard
Shows a summary of owned courses, upcoming schedules, and registration stats.

#### 3.2: Create New Course
Instructors can add new courses by providing details such as name, description, tuition, and capacity.

#### 3.3: Edit/Delete Course
Full control over their own courses including editing or deleting them.

#### 3.4: View Enrolled Students
Each course page shows a list of students who have registered.

#### 3.5: Update Instructor Profile
Instructors can edit their name, avatar, and credentials.

---

### 4. 🛡️ Admin Interface

#### 4.1: Admin Dashboard
A centralized panel displaying platform-wide metrics such as:
- Total number of students/instructors
- Course counts
- Popular courses

#### 4.2: Manage Courses
Full CRUD access to all courses on the platform. Admins can edit or delete any course.

#### 4.3: Manage Users
Admins can manage both students and instructors:
- View user lists
- Activate/deactivate accounts
- Reset passwords

#### 4.4: Manage Enrollments
Admins can see all course enrollments and manually unregister users if needed.

#### 4.5: Manage Categories
Create or update course categories for better organization.

#### 4.6: View Platform Logs (Optional)
Admins can audit changes made across the system for security and transparency.

#### 4.7: Admin Profile
Admins can edit their own profile and change credentials.

---

### 📸 Screenshot Examples

> *(Add screenshots here using Markdown `![Alt text](image-url)` if hosted on GitHub or externally)*


