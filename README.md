DB-Setup
======

1. Sicherstellen dass SQL Server 2012 oder älter verwendet wird.
2. SQL Server Configuration Manager aufmachen und bei SQL Server-Dienste die Eigenschaften des SQL Server (MSSQLSERVER) öffnen.
3. Zum Tab FileStream navigieren und alle Checkboxen setzen.
4. SQL Server (MSSQLSERVER) neu starten.
5. FileGroup und File erstellen.

ALTER database ProStudCreator
ADD FILEGROUP fsfg_ProStudCreator
CONTAINS FILESTREAM
GO

ALTER database ProStudCreator
ADD FILE
(NAME= 'fs_ProStudCreator',
FILENAME = 'C:\Users\FlavioMueller\Source\Repos\ProStudCreator\fs_ProstudCreator')
TO FILEGROUP fsfg_ProStudCreator
GO

6. Tabelle mit FileStream muss eine Spalte mit uniqueidentifier als ROWGUIDCOL haben.
7. Auf Tabellen Ebene ausführen: FILESTREAM_ON [fsfg_ProStudCreator]
8. Filestream funktioniert nur mit integrated Security (Windows autentifizierung.)
-> SQL Login muss Windows User sein.
9. Sicherstellen das der Windows-User zugriffsrechte auf den angegebenen Pfad hat.



Telerik-Setup
======

[Anleitung von Telerik](http://docs.telerik.com/devtools/aspnet-ajax/general-information/adding-the-telerik-controls-to-your-project)
