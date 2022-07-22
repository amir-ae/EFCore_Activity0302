CREATE OR ALTER PROCEDURE dbo.GetItemsForListing
    @minDate DATETIME = '2020.01.01',
    @maxDate DATETIME = '2022.12.31'
AS
BEGIN
    SET NOCOUNT ON;
    SELECT item.[Name], item.[Description], item.Notes, item.isActive, 
    		item.isDeleted, cat.[Name] CategoryName
    FROM dbo.Items item
    LEFT JOIN dbo.Categories cat on item.CategoryId = cat.Id
    WHERE (@minDate IS NULL OR item.CreatedDate >= @minDate)
    AND (@maxDate IS NULL OR item.CreatedDate <= @maxDate)
END
GO