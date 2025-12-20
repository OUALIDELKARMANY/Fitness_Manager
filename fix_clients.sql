-- Assigner tous les clients sans coach au premier coach disponible
UPDATE Utilisateurs AS c
INNER JOIN (
    SELECT Id FROM Utilisateurs 
    WHERE Discriminator = 'Coach' 
    LIMIT 1
) AS coach ON 1=1
SET c.CoachId = coach.Id
WHERE c.Discriminator = 'Client' 
  AND c.CoachId IS NULL;
