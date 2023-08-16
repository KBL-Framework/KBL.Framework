# KBL.Framework
Simple generic SOLID netstandard2.1 architectural framework that will be CRUD and map for you.

[![Nuget version (KBL.Framework)](https://img.shields.io/nuget/v/KBL.Framework)](https://www.nuget.org/packages/KBL.Framework) 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KBL-Framework_KBL.Framework&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=KBL-Framework_KBL.Framework)

___

## Data Access Layer
DAL is diversed for quering and manipulating (create / update / delete).

### Manipulating
Base is: 

 #### Example - I need create some data
   1. Create DLL project **xxx.Entities**
   1. Folder **Model**
   1. Create my model class - eg. **User**
      1. Inherit one from two classes
         - BaseEntity
         - AuditableEntity


## Business Access Layer

## Config section

###SqlConnection.BeginTransaction Method
KBL.Framework.DAL.Config:Transaction:IsolationLevel	- [https://docs.microsoft.com/cs-cz/dotnet/api/system.data.sqlclient.sqlconnection.begintransaction?view=netframework-4.8](https://docs.microsoft.com/cs-cz/dotnet/api/system.data.sqlclient.sqlconnection.begintransaction?view=netframework-4.8)


## Workshop 1 CZ
 - https://youtu.be/YOIKTBo6Esk
___

NuGet package: [https://www.nuget.org/packages/KBL.Framework/](https://www.nuget.org/packages/KBL.Framework/) 
