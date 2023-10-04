SELECT Proced.IDproc, Proced.Name, Proced.Price, Patient.First_name, Patient.Last_name, 
	Equipment.Name, Staff.Name, Staff.Last_Name, Medicine.Name FROM Proced 
	INNER JOIN Patient ON Proced.Patient_ID = Patient.IDpat
	INNER JOIN Equipment ON Proced.Equipment_ID = Equipment.IDeq
	INNER JOIN Staff ON Proced.Staff_ID = Staff.IDstaff
	INNER JOIN Medicine ON Proced.Medicine_ID = Medicine.IDmed;
