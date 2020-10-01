# BookCrossingBackEnd   [![License: MIT](https://img.shields.io/badge/license-MIT-ff69b4)](https://github.com/ita-social-projects/Bookcrossing-Back-End/blob/develop/LICENSE) [![Build Status](https://travis-ci.org/ita-social-projects/Bookcrossing-Back-End.svg?branch=develop)](https://travis-ci.org/ita-social-projects/Bookcrossing-Back-End) [![Build number](https://img.shields.io/badge/build-number-blue.svg)](https://travis-ci.org/github/ita-social-projects/Bookcrossing-Back-End/builds) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects-bookcrossing-back-end&metric=alert_status)](https://sonarcloud.io/dashboard?id=ita-social-projects-bookcrossing-back-end) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ita-social-projects-bookcrossing-back-end&metric=coverage)](https://sonarcloud.io/dashboard?id=ita-social-projects-bookcrossing-back-end) 

Platform for book crossing between company employees
[Website](https://book-crossing-web.azurewebsites.net/)  
  
## Git Flow  
We are using simpliest github flow to organize our work:  
![Github flow](https://scilifelab.github.io/software-development/img/github-flow.png)  
We have **master** , **develop** and **feature** branches.   
All features must be merged into develop branch!!!
Only the release should merge into the main branch!!!

## Getting Started
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes using docker containers. 

### Prerequisites
[Docker](https://www.docker.com) version 17.05 or higher

[Docker-compose](https://github.com/docker/compose)

###### Note: It's better to use [docker-desktop](https://www.docker.com/products/docker-desktop) if you are on windows

### Installing
1. Clone repository from GitHub with $ git clone https://github.com/ita-social-projects/Bookcrossing-Back-End.git

2. Move to Bookcrossing-Back-End and execute "docker-compose up"

###### Note: Also check Bookcrossing-Back-End/src/BookCrossingBackEnd/examplesettings.json and save it as appsettings.json if you want to deploy without docker-compose or pass any extra variables!
  
## Contribution rules: 
You're encouraged to contribute to our project if you've found any issues or missing functionality that you would want to see. Here you can see [the list of issues](https://github.com/ita-social-projects/Bookcrossing-Back-End/issues) and here you can create [a new issue](https://github.com/ita-social-projects/Bookcrossing-Back-End/issues/new/choose).

Before sending any pull request, please discuss requirements/changes to be implemented using an existing issue or by creating a new one. All pull requests should be done into `dev` branch.

Though there are two GitHub projects: [Bookcrossing-Back-End](https://github.com/ita-social-projects/Bookcrossing-Back-End) for back-end part and [BookCrossing-Front-End](https://github.com/ita-social-projects/Bookcrossing-Front-End) for front-end part) all of the requirements are listed in the first one - [Bookcrossing-Back-End](https://github.com/ita-social-projects/Bookcrossing-Back-End). 

Every pull request should be linked to an issue. So if you make changes on front-end part you should create an issue there (subtask) with a link to corresponding requirement (story, task or epic) on back-end.

All Pull Requests should start from prefix *#xxx-yyy* where *xxx* - task number and and *yyy* - short description 
e.g. #020-CreateAdminPanel  
Pull request should not contain any files that is not required by task.  
