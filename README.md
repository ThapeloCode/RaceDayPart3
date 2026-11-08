# RaceDay - Event Management System (Part 3 POE)

## Project Overview
**RaceDay** is a comprehensive South African road running, walking, and cycling event management system designed to seamlessly coordinate event participation, categories, route details, secure user registration, and official race results. The platform supports two primary user roles: **Organisers** and **Participants**.

---

## Repository Structure & Part 1 Deliverables
All initial planning documents and database setup scripts are stored in the `docs/` directory:
* **Section A (ERD):** Entity Relationship Diagram (`/docs/Section A (ERD).pdf`)
* **Section B (API Endpoint Plan):** Detailed specification of RESTful routes, request payloads, and expected responses (`/docs/Section B (ENDPOINT).pdf`)
* **Section C (Database Script):** Fully relational T-SQL database setup script (`/docs/RaceDay.sql`)
* **PoE Answer Sheet:** Completed project response documentation (`/docs/POE Answersheet.pdf`)

---

## CI/CD Pipeline & Build Validation
This repository utilizes **GitHub Actions** for automated build validation and structure checks. 

* **Workflow Path:** `.github/workflows/main.yml`
* **Build Status:** Passing (Green checkmark verified on the `master` branch)

![CI/CD Build Status](actions-build-green.png)

---

## Getting Started & Setup Instructions
1. Clone the repository:
   ```bash
   git clone [https://github.com/ThapeloCode/RaceDayPart3.git](https://github.com/ThapeloCode/RaceDayPart3.git)