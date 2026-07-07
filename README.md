# 💊 Pharmacy Management System

A full-featured Pharmacy Management System that simulates real-world pharmacy operations.  
Built to help pharmacists manage medications, prescriptions, patients, and suppliers efficiently, with a strong focus on **security, performance, and scalability**.

---

## 🚀 Key Features

### 🔐 Authentication, Security & Rate Limiting

The system enforces strict corporate-level security patterns within the **Infrastructure Layer** to ensure data integrity, mitigate automated attacks, and protect user accounts:

* **Context-Aware Brute-Force & Device Protection:** Implements a sophisticated security filter that monitors authentication patterns. If a device repeatedly enters incorrect passwords while attempting to log into a valid, registered account, the system triggers a dynamic, temporary device block to halt the attack.
* **IP & Device-Level Rate Limiting:** Rate limiters restrict excessive authentication requests at both the network and device levels. This entirely mitigates credential stuffing and distributed brute-force attacks before they hit the core business logic.
* **Secure Password Recovery Workflow:** Features a secure "Forgot Password" lifecycle. It integrates **Brevo** as an external SMTP service to securely dispatch time-bound verification and password reset tokens to users.
* **Architectural Separation:** All core security frameworks and persistence logic reside cleanly inside the **Infrastructure Layer**, while specialized authorization and role-limiting filters are applied directly at the **Presentation Layer (MVC)** to safeguard endpoints.

---

### 🤖 AI-Powered Features
- **Sentiment Analysis (NLP)**  
  Analyzes customer feedback using a custom NLP model to detect positive and negative reviews, enabling better decision-making and service improvement

- **Performance Optimization**  
  Implemented in-memory caching to reduce repeated NLP processing and improve response time

---
### 🤖 AI Assistant (Mistral Integration)

Integrated a conversational AI assistant powered by **Mistral AI**, connected through a **FastAPI service** that communicates seamlessly with the **ASP.NET Core backend**.

#### 🧠 Pharmacist Support System
Enables pharmacists to ask medical-related queries, retrieve drug information, and receive intelligent, AI-driven suggestions in real time.

#### 🔗 Backend Communication (C# ↔ Python)
Implemented a REST-based communication layer between the **C# application** and the **FastAPI AI service**, ensuring efficient request/response handling and smooth integration.

#### 💡 Use Cases
- Drug usage guidance  
- General medical inquiries  
- Decision-making assistance within the system
---

  ### 🤖 AI Model Details

- **Model:** Arabic Sentiment Analysis (Fine-Tuned BERT)
- **Source:** https://huggingface.co/kerolos1/analysis-of-Egyptian-sentiments

- **Description:**  
  A fine-tuned transformer-based model built on AraBERT architecture, designed to classify Arabic text into sentiment categories (positive / negative).  
  The model is adapted from a pre-trained Arabic sentiment model and further fine-tuned to improve accuracy on domain-specific data. :contentReference[oaicite:0]{index=0}

- **How it works:**  
  The model processes customer feedback and returns:
  - Sentiment label (Positive / Negative)
  - Confidence score

  📸 **Model in Action:**
  ![Sentiment Analysis](https://github.com/Kerolos247/MVC-Pharmacy/blob/master/Screenshot%202026-03-25%20032648.png)

---

### 💊 Medication Management
- Full CRUD operations for medications
- Expiration tracking and validation
- Low-stock and near-expiry alerts

---

### 📝 Prescription Management
- Add, search, and dispense prescriptions
- Online prescription upload (Cloud-based storage)
- Pagination for efficient data handling
- Automatic pricing calculation with dynamic discounts

---

### 🧑‍🤝‍🧑 Patient & Supplier Management
- Manage patients and suppliers with full CRUD functionality

---

### 📊 Dashboard
- Real-time statistics for patients, medications, suppliers, and prescriptions
- Data scoped to the logged-in pharmacist

---

### ⚠️ Smart Validation & Alerts
- Prevents invalid or expired data
- System-wide validation and real-time alerts

---

## 🛠 Tech Stack

- ASP.NET Core (MVC)
- Entity Framework Core
- SQL Server
- LINQ
- ASP.NET Identity
- JavaScript
- FastAPI (Python - AI Integration)
- Cloudinary (File Uploads)
- IMemoryCache (Performance Optimization)

---

## 🏗 Architecture & Design

- Clean Architecture & MVC Pattern
- Dependency Injection for loose coupling
- Scalable and maintainable design

---

## 🧩 Design Patterns

- **Repository Pattern & Unit of Work**  
  To abstract data access logic and manage database transactions efficiently.

- **Strategy Pattern**  
  For implementing dynamic discount logic with flexible and extensible behavior.

- **Factory Pattern**  
  For centralized and flexible object creation (e.g., invoice generation).

- **Dependency Injection (DI)**  
  To achieve loose coupling, improve testability, and enhance maintainability.

---

## 🎥 Demo

A working demo showcasing:
- Password reset workflow
- Brute-force protection
- Email notification system

🔗 https://drive.google.com/file/d/14wziabgBlKxHNud8WYIalyb1ykSPFlPR/view?usp=sharing
