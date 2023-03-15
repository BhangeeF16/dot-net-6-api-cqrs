CREATE PROCEDURE [dbo].[fp_GetUserUnSubscribedPantryItems]
@userID INT = 1,
@categoryID INT = 1,
@keyword VARCHAR(100) = '',
@pageNumber INT = 1,
@pageSize INT = 10,
@sortingCol VARCHAR(100) = 'ID',
@sortDirection VARCHAR(100) = 'ASC',
@totalCount INT OUTPUT
AS
BEGIN

SET @categoryID = CASE WHEN (@categoryID IS NULL OR @categoryID = 0) THEN 0 ELSE @categoryID END 
SET @keyword = CASE WHEN (@keyword IS NULL) THEN '' ELSE @keyword END 
SET @sortDirection = CASE WHEN (@sortDirection IS NULL OR @sortDirection = '' ) THEN 'ASC' ELSE @sortDirection END
SET @sortingCol = CASE WHEN (@sortingCol IS NULL OR @sortingCol = '') THEN 'ID' ELSE @sortingCol END 
SET @pageNumber = CASE WHEN (@pageNumber IS NULL OR @pageNumber = 0) THEN 1 ELSE @pageNumber END 

DECLARE @OrderByClause NVARCHAR(MAX) 
DECLARE @WhereClause NVARCHAR(MAX) 
DECLARE @ItemsSql NVARCHAR(MAX) 
DECLARE @CountSql NVARCHAR(MAX) 
DECLARE @SqlStatement NVARCHAR(MAX) 
DECLARE @TotalRowCount INT

SET @CountSql = 'SELECT @TotalRowCount = COUNT(*) ';
SET @ItemsSql = '
SELECT
	p.ID, p.Name, p.Description, p.IsOrganic, p.ItemType, 
	p.Price, p.ImagePath, p.IsActive, 
	p.fk_CategoryID AS CategoryID, pc.Name AS CategoryName, 
	pv.ShippingFrequency, pv.Frequency ';
SET @WhereClause = 	'
FROM dbo.PantryItem AS p
INNER JOIN dbo.PantryItemCategory AS pc ON p.fk_CategoryID = pc.ID
INNER JOIN dbo.PantryItemVariation AS pv ON pv.fk_PantryItemID = p.ID
LEFT JOIN UserPantryItem AS up ON p.ID = up.fk_PantryItemVariationID AND up.fk_UserID = ' + CAST(@userID AS VARCHAR(MAX)) + '
WHERE (up.fk_UserID IS NULL) 
	AND ((' + CAST(@categoryID  AS VARCHAR(MAX)) + ' = 0) OR (p.fk_CategoryID = ' + CAST(@categoryID  AS VARCHAR(MAX)) + '))
	AND ((p.IsDeleted = 0) AND (p.IsActive = 1))
	AND ((pc.IsDeleted = 0) AND (pc.IsActive = 1))
	AND
	(
		(''' + @keyword + ''' IS NULL OR ''' + @keyword + ''' = '''') 
		OR (p.Name LIKE ''%'' + ''' + @keyword + ''' + ''%'')
		OR (pc.Name LIKE ''%'' + ''' + @keyword + ''' + ''%'')
		OR (p.Description LIKE ''%'' + ''' + @keyword + ''' + ''%'')
		OR (p.Price LIKE ''%'' + ''' + @keyword + ''' + ''%'')
	)
';



SET @OrderByClause =		
		' ORDER BY ''' + @sortingCol + ''' ' + @sortDirection ;

IF @pageSize > 0 
BEGIN
	SET @OrderByClause = @OrderByClause + ' OFFSET(' + CAST(@pageNumber AS VARCHAR(MAX)) + ' - 1) * ' + CAST(@pageSize AS VARCHAR(MAX)) + 
		' ROWS FETCH NEXT ' + CAST(@pageSize AS VARCHAR(MAX)) + ' ROWS ONLY';
END

-- DATA
SET @SqlStatement = @ItemsSql + @WhereClause + @OrderByClause;
EXEC(@SqlStatement);
-- Paging DATA
SET @SqlStatement = @CountSql + @WhereClause;
EXEC sp_executesql @SqlStatement, N'@TotalRowCount INT OUTPUT', @totalCount OUTPUT


END