# Concept


### Database constraints

The database must contain at least one  and a maximum of one project with IsMainVersion = true.

###### Pros
    - throws an exception as soon as there is more or less than one mainversion
    
###### Cons
    - changes of the database scheme may require the constraints to be updated.

###### My thoughts on this

I think this is a really good and efficient way to detect errors quickly




### Unittests

###### Pros
    - automated
    
###### Cons
    - time-consuming
    - alot of work
    - not everything can be tested
    - doesn't presume that the page / code works in the end

###### My thoughts on this 

I don't like Unittest as they are very time-consuming and even more so if you have to test a GUI.




### Exception reporting

If an exception occures, an Email gets sent to the developer with additional information like page, projectId, user, stacktrace etc. 

###### Pros
    - The developer gets notified instantly and can react very fast.
    
###### Cons
    - The developer might get spammed with emails

###### My thoughts on this

I think this could be very useful as long as there are only very few exceptions occuring.
    
  
