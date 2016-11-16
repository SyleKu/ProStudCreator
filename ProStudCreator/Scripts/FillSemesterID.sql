UPDATE p SET SemesterId = 
(SELECT TOP 1 s.Id FROM Semester s WHERE s.StartDate > p.PublishedDate ORDER BY s.StartDate)
FROM Projects p;