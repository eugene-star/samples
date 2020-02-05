SELECT p.Name, c.Name
FROM Product p 
	LEFT JOIN ProductCategory pc ON pc.ProductId = p.Id
	LEFT JOIN Category c ON c.Id = pc.CategoryId