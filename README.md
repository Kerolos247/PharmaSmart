# 💊 PharmaSmart — Pharmacy Management System

An end-to-end Pharmacy Management System that models real-world pharmacy operations. PharmaSmart helps pharmacists manage medications, prescriptions, patients, and suppliers while integrating AI-powered services for intelligent pharmaceutical assistance.

## 🧠 AI Engine

The intelligent capabilities of PharmaSmart—including FDA-backed drug consultation, Egyptian Arabic voice assistance, sentiment analysis, and complaint classification—are powered by a decoupled, containerized Python microservices architecture.

👉 **Explore the PharmaSmart AI Microservices Repository:**  
https://github.com/Kerolos247/PharmaSmart-AI-Microservices

---

# 📋 Overview

PharmaSmart is a full-featured pharmacy management platform that combines modern backend engineering practices with AI-powered capabilities. The system enables pharmacists to manage medicines, prescriptions, patients, suppliers, and inventory through a unified ASP.NET Core MVC application, while dedicated Python microservices provide intelligent pharmaceutical services. The ASP.NET Core backend orchestrates AI requests and implements semantic caching to reduce LLM latency, token usage, and repeated requests before forwarding requests to the AI microservices.

## Key Capabilities

* 🔐 **Multi-Layered Security** — Brute-force protection, IP/device rate limiting, secure password recovery, and role-based authorization.
* 🏥 **Complete Pharmacy Operations** — Patient, supplier, prescription, and medicine lifecycle management.
* 📦 **Inventory Concurrency Control** — Dual-strategy concurrency handling using both pessimistic and optimistic locking.
* 🤖 **AI Integrations** — FDA-backed RAG chatbot, Egyptian Arabic voice assistant, and automated complaint & sentiment analysis.
* 📧 **Automated Inventory Monitoring** — Low-stock notifications and scheduled weekly shortage reports.

---

# ✨ Features

## 🔐 Authentication & Security

* Brute-force protection with temporary device blocking after repeated failed login attempts.
* IP-level and device-level rate limiting to mitigate credential stuffing and abuse.
* Secure password recovery using Brevo SMTP with time-limited verification codes.
* Role-based authorization using ASP.NET Core Identity.

---

## 🏥 Pharmacy Operations

* Complete Patient Management (CRUD).
* Supplier Management with relational medicine mapping.
* Medicine Catalog Management.
* Prescription registration and fulfillment workflow.
* Dynamic search across patients, suppliers, medicines, and prescriptions.
* Cloudinary integration for prescription image storage.

---

## 📦 Inventory & Concurrency Control

* **Pessimistic Locking** using SQL Server row-level locks (`UPDLOCK`, `ROWLOCK`) for safe medicine dispensing.
* **Optimistic Locking** using `RowVersion` concurrency tokens for concurrent updates.
* Automated low-stock detection and inventory shortage reporting.

---

## 🤖 AI-Powered Features

* FDA Drug Consultation using a Stateful RAG pipeline.
* Egyptian Arabic Voice Assistant powered by Whisper.
* Fine-tuned BERT models for sentiment analysis and complaint classification.
* **Semantic Caching implemented in the ASP.NET Core backend** to reduce LLM latency, token usage, and repeated requests before forwarding AI requests to the Python microservices.

---

# 🛠️ Tech Stack

| Category | Technologies |
| :--- | :--- |
| **Backend** | ASP.NET Core 8 (MVC), Entity Framework Core 8, LINQ, ASP.NET Core Identity |
| **Database** | SQL Server, Qdrant (Vector Database) |
| **AI Microservices** | FastAPI, Python 3.11, Docker, LangChain, Llama 3.3 (Groq API), Whisper, BERT |
| **Cloud Services** | Cloudinary, Brevo SMTP, Hugging Face |
| **Frontend** | Razor Views, Bootstrap, JavaScript (ES6+) |

---

# 🏗️ Architecture

## System Architecture

![C4 Architecture Diagram](https://github.com/Kerolos247/PharmaSmart/blob/master/Diagram-C4.png)

---

# 🎥 Demo

Watch the complete system walkthrough demonstrating:

* Inventory management
* Prescription workflow
* AI chatbot
* Voice assistant
* Concurrency handling
* Automated inventory alerts

👉 **Demo Video:**  
https://drive.google.com/file/d/1zOMZyWdEbH-0MTeFAaWENsiTbNgLzKmx/view?usp=drive_link

---

# 📦 Repository Notice

This repository is published for portfolio and code review purposes.

Configuration files, external service credentials, and deployment-specific settings have been intentionally excluded.

The project integrates several external services, including SQL Server, Cloudinary, Brevo SMTP, Groq API, and containerized AI microservices, which require additional configuration before the application can be executed.

---

# 👨‍💻 Author

* **LinkedIn:** https://linkedin.com/in/kerolos-adel-190948375
