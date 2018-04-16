# Concept

### Unittests
Writing Unittests for all methods if possible. Some methods like the PDF generation method are very difficult to test and can be ignored. The unittests have to executed before commiting and publishing.

###### Pros
    - automated
    
###### Cons
    - time-consuming
    - alot of work
    - not everything can be tested
    - doesn't mean that the page / code works in the end



### Exception reporting

If an exception occures, a notification should get sent to the developer, if possible with additional information like page, projectId, user and stacktrace. There are multiple good and free frameworks like NLog, Serilog and elmah and an older one called log4net. All uncaught exceptions of the .NET Appliction go through here: "Global.asax" Application_Error Event (https://msdn.microsoft.com/en-us/library/24395wz3.aspx). Thats where such a framework could be plugged in.

###### Pros
    - The developer gets notified instantly and can react very fast.
    
###### Cons
    - The developer might get spammed with emails
    
  

### Database constraints

The database must contain at least one  and a maximum of one project with IsMainVersion = true. This could be done with a contstraint in SQL(CHECK, UNIQUE,...) or in the code where it would have to be checked in "AddNewProject.aspx?Id=xy" where it runs a check that there is only one project where the mainversion is "xy" and if not then throw an exception. The exception could then be sent to the developer. Also should the ProjectNr be checked, so that there is only one unique ProjectNr per institue and per semester. This could be easily done with a UNIQUE constraint in the db.

###### Pros
    - if the check happens in the db: throws an exception as soon as you try to insert something wrong into the db
    
###### Cons
    - changes of the database scheme may require the constraints to be updated.
    - if the check would be done in the code, then it wouldn't be clear where the error happened. The stacktrace would report the line where we threw the exception ourself.
    



  
