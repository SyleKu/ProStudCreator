

ALTER database ProStudCreator

ADD FILEGROUP fsfg_ProStudCreator

CONTAINS FILESTREAM

GO


ALTER database ProStudCreator

ADD FILE

(

    NAME= 'fs_ProStudCreator',

    FILENAME = 'C:\Users\FlavioMueller\Source\Repos\ProStudCreator\fs_ProstudCreator'

)

TO FILEGROUP fsfg_ProStudCreator

GO


use [aspnet-ProStudCreator-20140818043155] 

GO
Alter Table Projects ADD ROWGUID uniqueidentifier Null

GO

Insert Into Projects (ROWGUID)
VALUES (NEWSEQUENTIALID())

GO

ALTER TABLE Projects ALTER COLUMN ROWGUID uniqueidentifier NOT NULL

GO

ALTER TABLE Projects
ALTER COLUMN ROWGUID ADD ROWGUIDCOL
GO

 
 ALTER TABLE Projects

SET ( FILESTREAM_ON = fsfg_ProStudCreator)

GO

ALTER TABLE Projects

ADD ProjectDeliveryData varbinary(MAX) FILESTREAM NULL;

GO