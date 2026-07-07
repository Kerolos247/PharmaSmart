# 💊 PharmaSmart — Pharmacy Management System

An enterprise-grade Pharmacy Management System that models real-world pharmacy operations end to end. PharmaSmart helps pharmacists manage medications, prescriptions, patients, and suppliers efficiently, with a design that prioritizes **security, performance, and scalability**.

---

## 🚀 Key Features

### 🔐 Authentication, Security & Rate Limiting

The Infrastructure Layer implements a set of security controls designed to protect user accounts and preserve data integrity against automated and distributed attacks:

* **Context-Aware Brute-Force & Device Protection** — A dedicated security filter monitors authentication attempts and applies a dynamic, temporary device block when repeated failed logins are detected against a valid, registered account.
* **IP & Device-Level Rate Limiting** — Rate limiters constrain authentication requests at both the network and device level, reducing exposure to credential stuffing and distributed brute-force attempts before they reach core business logic.
* **Secure Password Recovery Workflow** — A "Forgot Password" flow issues time-bound verification and reset tokens, delivered via **Brevo** as the external SMTP provider.
* **Architectural Separation** — Security frameworks and persistence logic live in the **Infrastructure Layer**, while authorization and role-based access filters are enforced at the **Presentation Layer (MVC)**, keeping each concern in its proper place.

---

### 🤖 AI-Powered Features & Microservices Architecture

Computationally intensive AI workloads are isolated from core pharmacy operations in a dedicated **AI Microservices Layer**, built with **FastAPI (Python)** and containerized with **Docker**. Each microservice follows the single-responsibility principle, communicates with the ASP.NET MVC core exclusively over **RESTful APIs**, and is deployed on **Hugging Face**.

#### 1. Pharmacist Consultation Service (Stateful RAG Microservice)
Provides evidence-based pharmaceutical consultation through a stateful **Retrieval-Augmented Generation (RAG)** pipeline backed by a persistent vector knowledge base.

* **FDA Semantic Knowledge Base** — Converts official FDA pharmaceutical documentation into vector embeddings using **Sentence Transformers**.
* **Vector Storage** — Stores and indexes embeddings in a **Qdrant Vector Database** for high-performance semantic similarity search.
* **Retrieval Pipeline:**
  1. Receive the pharmacist's question.
  2. Generate a semantic embedding for the query.
  3. Search Qdrant for the most relevant FDA passages.
  4. Rerank candidates using **FlashRank** to improve retrieval precision.
  5. Build an augmented prompt from the reranked context.
  6. Generate a context-grounded answer using **Llama 3.3** via the **Groq API**.
  7. Return the answer together with its supporting source evidence.
* **Embedding API** — Exposes an embedding endpoint consumed by the ASP.NET MVC application to power the custom Semantic Cache.

#### 2. Customer Service Voice Assistant (Stateless AI Microservice)
An intelligent, stateless assistant that handles operational and customer-service inquiries on the web portal.

* **Egyptian Arabic Optimization** — Tuned to understand spoken Egyptian Arabic, allowing patients to interact naturally in their local dialect.
* **Speech-to-Text (STT)** — Transcribes Egyptian Arabic audio using **Whisper**.
* **Response Generation** — Produces natural-language replies with **Llama** and converts them to speech via Text-to-Speech.
* **Supported Queries** — Branch locations, contact details, business hours, available services, site navigation help, and general support.
* **Horizontal Scalability** — Runs as a stateless service, allowing scaling through simple container replication.

#### 3. Sentiment & Complaint Classification Service (Stateless AI Microservice)
A text-analysis pipeline embedded in the customer feedback flow to monitor service quality.

* **BERT Classification Model** — Routes submissions to a fine-tuned BERT model for preprocessing and text understanding.
* **Dual Classification** — Performs sentiment analysis (positive/negative) alongside topic classification.
* **Structured Output** — Maps complaints to operational categories (e.g., *delivery delay, missing medication, incorrect order, staff behavior, website issues, payment problems*) and returns them as structured JSON to the .NET core application.

#### 4. Custom C# Semantic Caching Component
A hybrid caching layer built natively into the C# codebase to optimize AI-layer performance.

* **Token & Cost Reduction** — Intercepts outgoing AI queries and checks for semantically similar historical requests.
* **Latency Reduction** — Serves cached responses for semantically matching queries, cutting redundant LLM token usage, API latency, and unnecessary microservice compute.
* **Performance Optimization** — Uses in-memory caching to reduce repeated NLP processing and improve response times.

---

### 🖥️ Core Backend & Architecture (.NET)

The backend is an enterprise-grade **ASP.NET Core MVC** application built on **Clean Architecture** principles, ensuring maintainability, separation of concerns, and testability.

#### 🏗️ Architectural Layers
* **Domain Layer** — Core business entities, domain logic, and custom domain exceptions. Fully independent of external frameworks, databases, or UI concerns.
* **Application Layer** — Orchestrates use cases, encapsulates business rules, and defines DTOs and interface abstractions used across the application.
* **Infrastructure Layer** — Handles external concerns: data persistence via Entity Framework Core, external API integrations, and the security/authentication implementation.
* **Presentation Layer (MVC)** — The user-facing layer, structured into modular components that handle HTTPS requests, enforce role-based access control, and render views.

#### 🧩 Core Backend Components

* **Patient Lifecycle Management** — Comprehensive patient profiles with optimized search and full CRUD workflows.
* **Prescription & Fulfillment** — Tracks, updates, and searches medical prescriptions, with a dedicated checkout flow for fulfillment and payment.
* **Cloud-Decoupled Media Storage** — Handles prescription image uploads; transactional metadata is persisted in SQL Server while physical assets are offloaded to **Cloudinary** via background processes.
* **Catalog & Supplier Intake** — Links each medicine entry to its vendor and requires an initial batch shipment intake to populate live inventory counts when a new medicine is added.
* **Inventory & Concurrency Control:**
  * **Real-Time Stock Management** — Every sale or manual stock adjustment updates live inventory immediately, enforcing fulfillment rules.
  * **Pessimistic Concurrency Locking** — Uses SQL Server row-level locks during checkout to prevent race conditions such as double-selling or negative stock when multiple pharmacists dispense the same medicine concurrently.
* **Custom Semantic Caching Component** — Native C# middleware that intercepts duplicate queries, calls the FastAPI embedding endpoint, and caches semantically similar prior requests to reduce LLM token overhead and API latency.
* **Automated Operational Alerting:**
  * **Low-Stock Alerts** — Background workers send immediate notification emails via **Brevo** when a medicine's stock drops below its critical threshold.
  * **Weekly Shortage Reports** — A scheduled background service compiles and emails a weekly summary of depleted or critically low-stock medicines.

#### 🛠️ Core Backend Tech Stack
* **Framework:** .NET Core / ASP.NET Core MVC
* **Data Access & ORM:** Entity Framework Core (explicit transactions and row locks)
* **Primary Database:** SQL Server (relational data, transaction logs, row-level locking)
* **Third-Party Integrations:** Cloudinary (image hosting), Brevo (transactional email & reporting)

---

## 🛠️ Detailed Technology Stack

The platform follows a polyglot architecture, balancing high-throughput transactional integrity with efficient machine learning inference.

### 🖥️ Backend Frameworks & Core Ecosystem
* **ASP.NET Core 8 (MVC)** — Primary host for the enterprise web engine, providing dependency injection, secure routing, and separation of concerns.
* **FastAPI (Python 3.11)** — Asynchronous framework powering the AI microservices layer, chosen for its low overhead and native async support.
* **Docker** — Containerizes each Python AI microservice for environmental isolation and consistent deployment.

### 💾 Data Persistence, Querying & Concurrency
* **Entity Framework Core 8** — ORM managing migrations, Fluent API configuration, and database transactions.
* **SQL Server** — Relational database engine handling consistent tables, transaction logs, and row-level locks.
* **LINQ** — Type-safe, compiled queries used in the Application layer for efficient server-side data access.
* **Qdrant Vector Database** — Stores, indexes, and queries semantic embeddings for the RAG consultation pipeline.

### 🔐 Authentication & Identity
* **ASP.NET Core Identity** — Manages authentication, password hashing, persistent cookies, and role-based access control.

### 🤖 Machine Learning, NLP & AI Core
* **Llama 3.3 (via Groq API)** — Foundational LLM for context-grounded response generation in consultation and support pipelines.
* **Fine-Tuned BERT** — Task-specific model for sentiment analysis and multi-class complaint classification.
* **Whisper (OpenAI)** — Automatic speech recognition for transcribing Egyptian Arabic audio input.
* **Sentence Transformers & PyTorch** — Convert clinical text into semantic vector embeddings.
* **LangChain & FlashRank** — Orchestrate the RAG execution graph and apply context-aware reranking over retrieved passages.

### ☁️ Infrastructure & Third-Party Integrations
* **Cloudinary SDK** — Non-blocking cloud storage for prescription assets, offloaded from the application server.
* **Brevo API (formerly Sendinblue)** — SMTP provider for token delivery and automated reporting.
* **Microsoft.Extensions.Caching (`IMemoryCache`)** — In-memory storage backing the custom semantic cache.
* **JavaScript (ES6+)** — Powers responsive UI behavior, client-side validation, and asynchronous requests to backend controllers.
