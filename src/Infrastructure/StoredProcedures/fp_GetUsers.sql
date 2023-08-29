CREATE PROCEDURE [dbo].[fp_GetUsers]
@userID INT = NULL,
@roleFilter INT = NULL,
@keywordFilter INT = 0,
@keyword VARCHAR(100) = '',
@pageNumber INT = 1,
@pageSize INT = 10,
@sortingCol VARCHAR(100) = 'ID',
@sortDirection VARCHAR(100) = 'ASC',
@totalCount INT OUTPUT
AS
BEGIN

SET @keyword = CASE WHEN (@keyword IS NULL) THEN '' ELSE TRIM(@keyword) END;
SET @pageSize = CASE WHEN (@pageSize IS NULL OR @pageSize = 0) THEN 10 ELSE @pageSize END;
SET @pageNumber = CASE WHEN (@pageNumber IS NULL OR @pageNumber = 0) THEN 1 ELSE @pageNumber END;
SET @sortingCol = CASE WHEN (@sortingCol IS NULL OR @sortingCol = '') THEN 'u.ID' ELSE @sortingCol END;
SET @sortDirection = CASE WHEN (@sortDirection IS NULL OR @sortDirection = '' ) THEN 'DESC' ELSE @sortDirection END;
SET @roleFilter = CASE WHEN (@roleFilter IS NULL OR @roleFilter = 0 OR @roleFilter = '') THEN 1 ELSE @roleFilter END;
SET @userID = CASE WHEN (@userID IS NULL OR @userID = '') THEN 0 ELSE @userID END;

DECLARE 
@OrderByClause NVARCHAR(MAX),
@JoinClause NVARCHAR(MAX),
@WhereClause NVARCHAR(MAX),
@ItemsSql NVARCHAR(MAX),
@CountSql NVARCHAR(MAX),
@SqlStatement NVARCHAR(MAX),
@TotalRowCount INT;

SET @CountSql = 'SELECT @TotalRowCount = COUNT(*) FROM dbo.[User] AS u ';
SET @ItemsSql = '
SELECT 
	u.ID, u.FirstName, u.LastName, u.Email, 
	COALESCE(u.ChargeBeeCustomerID, '''') AS ChargeBeeCustomerID, 
	u.PhoneNumber AS Phone, u.fk_RoleID, r.Name AS RoleName, 
	COALESCE(us.ChargeBeeID , ''No Subscription Associated'') AS ChargeBeeSubscriptionID, 
	COALESCE(us.Status , 0) AS SubscriptionStatus, COALESCE(pv.InvoiceName, '''') AS PlanName 
FROM dbo.[User] AS u ';

SET @joinClause = '
INNER JOIN Role AS r ON r.ID = u.fk_RoleID 
										 AND (r.IsActive = 1 AND r.IsDeleted = 0)
										 AND (r.ID <> 1)
										 AND ((' + CAST(@roleFilter AS VARCHAR(2)) + ' = 1) OR (r.ID = ' + CAST(@roleFilter AS VARCHAR(2)) + '))
LEFT JOIN UserSubscription AS us on u.ID = us.fk_UserID AND (us.IsActive = 1 AND us.IsDeleted = 0)
LEFT JOIN SubscriptionSetting AS ss ON us.fk_CurrentSettingID = ss.ID
LEFT JOIN PlanVariation AS pv ON ss.fk_PlanVariationID = pv.ID ';

SET @WhereClause = ' 
WHERE 
	(u.IsDeleted = 0)
	AND ((' + CAST(@userID AS VARCHAR(2)) + ' > 0) AND (u.ID <> ' + CAST(@userID AS VARCHAR(2)) + '))
	AND ';
	
SET @WhereClause = @WhereClause + 
CASE 
	WHEN (@keywordFilter = 1) THEN '
	(''' + @keyword + ''' IS NULL OR ''' + @keyword + ''' = '''' OR  u.Email like  ''%'+ @keyword + '%'') '
	WHEN (@keywordFilter = 2) THEN '
	(''' + @keyword + ''' IS NULL OR ''' + @keyword + ''' = '''' OR us.ChargeBeeID =  ''' + @keyword + ''') '
	ELSE '
	(
		(''' + @keyword + ''' IS NULL OR ''' + @keyword + ''' = '''')
		OR CONCAT(u.FirstName , '' '' , u.LastName) LIKE ''%' + @keyword + '%''
		OR u.FirstName LIKE ''%' + @keyword + '%'' 
		OR u.LastName LIKE ''%' + @keyword + '%'' 
	) ' 
END;
										 
SET @OrderByClause = '
ORDER BY ' + @sortingCol + ' ' + @sortDirection ;

IF @pageSize > 0 
BEGIN
	SET @OrderByClause = @OrderByClause + ' OFFSET(' + CAST(@pageNumber AS VARCHAR(MAX)) + ' - 1) * ' + CAST(@pageSize AS VARCHAR(MAX)) + 
		' ROWS FETCH NEXT ' + CAST(@pageSize AS VARCHAR(MAX)) + ' ROWS ONLY';
END

-- DATA
SET @SqlStatement = @ItemsSql + @JoinClause + @WhereClause + @OrderByClause;
EXEC(@SqlStatement);

-- Paging DATA
SET @SqlStatement = @CountSql + @JoinClause + @WhereClause;
EXEC sp_executesql @SqlStatement, N'@TotalRowCount INT OUTPUT', @totalCount OUTPUT


END