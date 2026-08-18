# SW-MAP

[![.NET](https://img.shields.io/badge/.NET-ASP.NET_Core-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-4169E1?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Supabase](https://img.shields.io/badge/Supabase-Database_%26_Storage-3FCF8E?logo=supabase&logoColor=white)](https://supabase.com/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)
[![Azure](https://img.shields.io/badge/Azure-Container_Apps-0078D4?logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/)
[![GitHub Actions](https://img.shields.io/badge/GitHub_Actions-CI%2FCD-2088FF?logo=githubactions&logoColor=white)](https://github.com/features/actions)

SW-MAP is a web application for discovering and sharing outdoor street workout and calisthenics spots across Bulgaria.

Users can explore workout locations on an interactive map, view photos and details, find nearby spots, and contribute new workout spots to the community.

🌐 **Live:** [sw-map.eu](https://sw-map.eu)

## Screenshots

### Interactive Map

<img width="2524" height="1345" alt="image" src="https://github.com/user-attachments/assets/bfe21d45-5be0-421a-972d-db9856255229" />

### Workout Spot Details

<img width="2516" height="1342" alt="image" src="https://github.com/user-attachments/assets/e37a6a38-7708-480d-b53e-0d512f72eea7" />

### Add Workout Spot

<img width="2505" height="1346" alt="image" src="https://github.com/user-attachments/assets/4cb6e17a-73e4-4f7f-8d05-55ea9989a5ef" />

## Features

- Interactive map of street workout spots across Bulgaria
- Marker clustering for nearby locations
- Search for workout spots
- Find the nearest workout spot using the user's location
- Detailed spot pages with photos, location and equipment information
- User registration and email confirmation
- Add new workout spots
- Edit existing workout spots
- Image uploads
- Admin approval system for new spots and user-submitted changes
- Responsive design for desktop and mobile
- Light and dark themes

## Tech Stack

### Backend
- C#
- ASP.NET Core
- Razor Pages
- Entity Framework Core
- ASP.NET Core Identity

### Frontend
- HTML
- CSS
- JavaScript
- Bootstrap
- Leaflet
- Leaflet.markercluster

### Database & Storage
- PostgreSQL
- Supabase
- Supabase Storage

### Deployment & Infrastructure
- Docker
- Azure Container Apps
- GitHub Actions
- Custom domain and HTTPS
- Application monitoring
- Database and image storage backups

## How It Works

Workout spots are displayed on an interactive Leaflet map. Users can browse existing locations without an account, while registered users can contribute new spots, upload photos, and suggest changes to existing spots.

New locations and user-submitted edits go through an approval process before becoming publicly visible.

Images are stored in Supabase Storage, while application data is stored in PostgreSQL.

The application is containerized with Docker and deployed to Azure Container Apps through a GitHub Actions CI/CD workflow.

## Project Status

SW-MAP is live and actively being developed.

The application is currently being tested and used by real users, with workout spots being contributed from different locations across Bulgaria.
