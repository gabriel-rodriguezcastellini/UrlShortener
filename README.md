<img src="https://github.com/gabriel-rodriguezcastellini/UrlShortener/blob/main/img/shortener.jpeg" alt="URL Shortener logo" title="urlShortener" align="right" height="60" />

# URL Shortener

.NET Core application, based on a simplified microservice architecture and Docker containers.

## Status (GitHub Actions)

[![build](https://github.com/gabriel-rodriguezcastellini/UrlShortener/actions/workflows/build-validation.yml/badge.svg)](https://github.com/gabriel-rodriguezcastellini/UrlShortener/actions/workflows/build-validation.yml) [![CodeQL](https://github.com/gabriel-rodriguezcastellini/urlShortener/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/gabriel-rodriguezcastellini/urlShortener/actions/workflows/codeql.yml)

## Getting Started

Make sure you have [installed](https://docs.docker.com/docker-for-windows/install/) docker in your environment. After that, you can run the below commands from the **/UrlShortener/** directory and get started immediately.

```powershell
docker-compose build
docker-compose up
```

You should be able to browse different components of the application by using the below URLs :

```
Web Status : http://host.docker.internal:5052/
API :  http://host.docker.internal:5051/
Web Front-end :  http://host.docker.internal:5050/
Seq :  http://host.docker.internal:5341/
```

### Architecture overview

This application is cross-platform at the server and client-side, thanks to .NET 6 services capable of running on Linux or Windows containers depending on your Docker host.
The architecture proposes a microservice oriented architecture implementation using HTTP as the communication protocol between the client app and the microservice.

## Read further

- [Explore the application](https://github.com/gabriel-rodriguezcastellini/UrlShortener/wiki#explore-the-application)
