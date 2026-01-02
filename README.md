# NeoReporting – Crystal Reports Generator Service for Nea Egnatia Odos

## Overview

NeoReporting is a **.NET Framework 4.8 REST API service** that generates reports using  
**SAP Crystal Reports** and exports them to **PDF or Excel (XLSX)**.

The service:
- Accepts parameters and grouping options via REST
- Connects to an **Oracle database**
- Generates reports from `.rpt` templates
- Stores output files using **unique names**
- Writes logs using **NLog**

---

## Technology Stack

- .NET Framework 4.8
- ASP.NET Web API
- SAP Crystal Reports Runtime
- Oracle Database (TNS / EZCONNECT)
- NLog (file-based logging)
- JSON configuration (`appsettings.json`)

---

## Project Structure

