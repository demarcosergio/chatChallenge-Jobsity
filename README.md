#Chat challenge with bot and groups

### **To move forward with the challenge I did:**

- I started with the creation of the layers to use Clean architecture, leaving the following layers defined
  * API
  * core
  * Infrastructure
  * Quiz

I started with the creation of the ASP .Net Core web app project with .NET Version 6, and divided in three parts: WEB, Bot and the consumer. 
I also decided to use dependency injection since this will facilitate the coupling or decoupling of each module, having the freedom to change the database, the bot service, or the service connected to RabbitMQ

Regarding class architecture, It based mainly on Singleton and Composite, which allowed me to define that each service responds for itself with a unique responsibility and class inheritance .

**_To run the project:_**

You need to do the following:
Go to the folder where you have the docker-compose.yml and in the terminal or prompt and run
**docker-compose up**
After that go to http://localhost:8080, and remember to sign up to use the app.
