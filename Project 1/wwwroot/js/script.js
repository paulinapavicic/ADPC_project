// =========================
// Patients Section
// =========================


document.getElementById('patientForm').addEventListener('submit', async function (e) {
    e.preventDefault();

  
    
    const personalIdentificationNumber = document.getElementById('personalIdentificationNumber').value;
    const name = document.getElementById('name').value;
    const surname = document.getElementById('surname').value;
    const dateOfBirth = document.getElementById('dateOfBirth').value;
    const sex = document.getElementById('sex').value;

  
    const response = await fetch('https://localhost:7023/api/patients', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            personalIdentificationNumber,
            name,
            surname,
            dateOfBirth,
            sex
        })
    });

    if (response.ok) {
        alert('Patient added successfully!');
        document.getElementById('patientForm').reset();
        loadPatients(); 
    } else {
        alert('Failed to add patient.');
        console.error(await response.text());
    }
});

document.getElementById('loadPatients').addEventListener('click', loadPatients);

async function loadPatients() {
    const response = await fetch('https://localhost:7023/api/patients');

    if (response.ok) {
        const patients = await response.json();
        const tableBody = document.getElementById('patientsTableBody');

        
        tableBody.innerHTML = '';

      
        patients.forEach(patient => {
            // Format Date of Birth to YYYY-MM-DD
            const formattedDate = new Date(patient.dateOfBirth).toLocaleDateString('en-GB'); 


            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${patient.patientId}</td>
                <td>${patient.personalIdentificationNumber}</td>
                <td>${patient.name}</td>
                <td>${patient.surname}</td>
                <td>${formattedDate}</td> <!-- Display formatted date -->
                <td>${patient.sex}</td>
 <td>
                   <button class="edit-btn">Edit</button>
                    <button onclick="deletePatient(${patient.patientId})">Delete</button>
                </td>`; 
            tableBody.appendChild(row);

           
            row.querySelector('.edit-btn').addEventListener('click', function () {
                enableInlineEditing(row, patient);
            });
        });

    } else {
        alert("Failed to load patients");
    }
}


async function enableInlineEditing(row, patient) {
    // Replace each cell (except ID and Actions) with an input field
    row.innerHTML = `
        <td>${patient.patientId}</td>
        <td><input type="text" value="${patient.personalIdentificationNumber}" class="edit-pin"></td>
        <td><input type="text" value="${patient.name}" class="edit-name"></td>
        <td><input type="text" value="${patient.surname}" class="edit-surname"></td>
        <td><input type="date" value="${patient.dateOfBirth.slice(0, 10)}" class="edit-dob"></td>
        <td><input type="text" value="${patient.sex}" class="edit-sex" maxlength="1"></td>
        <td>
            <button class="save-btn">Save</button>
            <button class="cancel-btn">Cancel</button>
        </td>
    `;

    // Save logic
    row.querySelector('.save-btn').onclick = async function () {
        const updatedPatient = {
            personalIdentificationNumber: row.querySelector('.edit-pin').value,
            name: row.querySelector('.edit-name').value,
            surname: row.querySelector('.edit-surname').value,
            dateOfBirth: row.querySelector('.edit-dob').value,
            sex: row.querySelector('.edit-sex').value
        };
        const response = await fetch(`https://localhost:7023/api/patients/${patient.patientId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updatedPatient)
        });
        if (response.ok) {
            alert("Patient updated!");
            loadPatients();
        } else {
            alert("Failed to update patient.");
        }
    };

    // Cancel logic
    row.querySelector('.cancel-btn').onclick = function () {
        loadPatients();
    };
}





async function deletePatient(patientId) {
    const confirmDelete = confirm("Are you sure you want to delete this patient?");

    if (!confirmDelete) return;

    const response = await fetch(`https://localhost:7023/api/patients/${patientId}`, {
        method: 'DELETE'
    });

    if (response.ok) {
        alert("Patient deleted successfully!");
        loadPatients(); 
    } else {
        alert("Failed to delete patient.");
    }
}



// =========================
// Medical Records Section
// =========================


document.getElementById('medicalRecordForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    
    const patientId = document.getElementById('patientId').value;
    const diseaseName = document.getElementById('diseaseName').value;
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    
    const response = await fetch('https://localhost:7023/api/medicalrecords', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            patientId,
            diseaseName,
            startDate,
            endDate
        })
    });

    if (response.ok) {
        alert('Medical record added successfully!');
        document.getElementById('medicalRecordForm').reset();
        loadMedicalRecords(); 
    } else {
        alert('Failed to add medical record.');
        console.error(await response.text());
    }
});

document.getElementById('loadAllMedicalRecords').addEventListener('click', loadAllMedicalRecords);

async function loadAllMedicalRecords() {
    console.log("Fetching all medical records...");

    try {
        
        const response = await fetch('https://localhost:7023/api/medicalrecords'); 

        if (response.ok) {
            const medicalRecords = await response.json(); 
            console.log("Fetched all medical records:", medicalRecords); 

            const tableBody = document.getElementById('medicalRecordsTableBody'); 

           
            tableBody.innerHTML = '';

            medicalRecords.forEach(record => {
                console.log("Adding record to table:", record); 

                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${record.recordId}</td>
                    <td>${record.patientId}</td>
                    <td>${record.diseaseName}</td>
                    <td>${record.startDate}</td>
                    <td>${record.endDate}</td>
<td> <button class="edit-btn">Edit</button>
            <button onclick="deleteMedicalRecord(${record.recordId})">Delete</button>
        </td>`; 
                tableBody.appendChild(row);

                row.querySelector('button.edit-btn').addEventListener('click', () => {
                    enableInlineEditing(row, record);
                });

            });
        } else {
            alert("Failed to load medical records");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while fetching medical records:", error);
    }
}

async function enableInlineEditing(row, record) {
    // Replace each cell (except Record ID and Actions) with input fields
    row.innerHTML = `
        <td>${record.recordId}</td>
        <td><input type="number" value="${record.patientId}" class="edit-patientId"></td>
        <td><input type="text" value="${record.diseaseName}" class="edit-diseaseName"></td>
        <td><input type="date" value="${record.startDate ? record.startDate.slice(0, 10) : ''}" class="edit-startDate"></td>
        <td><input type="date" value="${record.endDate ? record.endDate.slice(0, 10) : ''}" class="edit-endDate"></td>
        <td>
            <button class="save-btn">Save</button>
            <button class="cancel-btn">Cancel</button>
        </td>
    `;

    // Save logic
    row.querySelector('.save-btn').onclick = async function () {
        const updatedRecord = {
            patientId: parseInt(row.querySelector('.edit-patientId').value),
            diseaseName: row.querySelector('.edit-diseaseName').value,
            startDate: row.querySelector('.edit-startDate').value,
            endDate: row.querySelector('.edit-endDate').value || null
        };

        const response = await fetch(`https://localhost:7023/api/medicalrecords/${record.recordId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updatedRecord)
        });

        if (response.ok) {
            alert("Medical record updated!");
            loadAllMedicalRecords(); // Reload the table
        } else {
            alert("Failed to update medical record.");
            console.error(await response.text());
        }
    };

    // Cancel logic
    row.querySelector('.cancel-btn').onclick = function () {
        loadAllMedicalRecords(); 
    };
}

 

        

async function deleteMedicalRecord(recordId) {
    const confirmDelete = confirm("Are you sure you want to delete this medical record?");

    if (!confirmDelete) return;

    try {
        const response = await fetch(`https://localhost:7023/api/medicalrecords/${recordId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("Medical record deleted successfully!");
            loadAllMedicalRecords(); 
        } else {
            alert("Failed to delete medical record.");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while deleting medical record:", error);
    }
}




// =========================
// Checkups Section
// =========================


document.getElementById('checkupForm').addEventListener('submit', async function (e) {
    e.preventDefault();

   
    const patientId = document.getElementById('checkupPatientId').value;
    const procedureCode = document.getElementById('procedureCode').value;
    const checkupDate = document.getElementById('checkupDate').value;

    
    const response = await fetch(`https://localhost:7023/api/checkups`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            patientId,
            procedureCode,
            checkupDate
        })
    });

    if (response.ok) {
        alert("Check-up added successfully");
        loadCheckups()
    }
})


document.getElementById('loadAllCheckups').addEventListener('click', loadAllCheckups);

async function loadAllCheckups() {
    console.log("Fetching all checkups..."); 

    try {
        
        const response = await fetch('https://localhost:7023/api/checkups');

        if (response.ok) {
            const checkups = await response.json(); 
            console.log("Fetched all checkups:", checkups);

            const tableBody = document.getElementById('checkupsTableBody');

           
            tableBody.innerHTML = '';

           
            checkups.forEach(checkup => {
                console.log("Adding record to table:", checkup); 

                
                const formattedDate = new Date(checkup.checkupDate).toLocaleDateString('en-US', {
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                });

                const formattedTime = new Date(checkup.checkupDate).toLocaleTimeString('en-US', {
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: true
                });

                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${checkup.checkupId}</td>
                    <td>${checkup.patientId}</td>
                    <td>${checkup.procedureCode}</td>
                    <td>${formattedDate} at ${formattedTime}</td>
<td>
            <button class="edit-btn">Edit</button>
            <button onclick="deleteCheckup(${checkup.checkupId})">Delete</button>
        </td>`; 
                tableBody.appendChild(row);

                row.querySelector('button.edit-btn').addEventListener('click', () => {
                    enableInlineEditing(row, checkup);
                });

            });
        } else {
            alert("Failed to load checkups");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while fetching checkups:", error);
    }
}

async function enableInlineEditing(row, checkup) {
    // Convert checkupDate to ISO date and time parts for inputs
    const datePart = checkup.checkupDate ? checkup.checkupDate.slice(0, 10) : '';
    const timePart = checkup.checkupDate ? checkup.checkupDate.slice(11, 16) : '';

    row.innerHTML = `
        <td>${checkup.checkupId}</td>
        <td><input type="number" value="${checkup.patientId}" class="edit-patientId"></td>
        <td><input type="text" value="${checkup.procedureCode}" class="edit-procedureCode"></td>
        <td>
            <input type="date" value="${datePart}" class="edit-date">
            <input type="time" value="${timePart}" class="edit-time">
        </td>
        <td>
            <button class="save-btn">Save</button>
            <button class="cancel-btn">Cancel</button>
        </td>
    `;

    // Save handler
    row.querySelector('.save-btn').onclick = async () => {
        const updatedCheckupDate = `${row.querySelector('.edit-date').value}T${row.querySelector('.edit-time').value}:00`;

        const updatedCheckup = {
            patientId: parseInt(row.querySelector('.edit-patientId').value, 10),
            procedureCode: row.querySelector('.edit-procedureCode').value,
            checkupDate: updatedCheckupDate
        };

        try {
            const response = await fetch(`https://localhost:7023/api/checkups/${checkup.checkupId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedCheckup)
            });

            if (response.ok) {
                alert('Checkup updated!');
                loadAllCheckups();
            } else {
                alert('Failed to update checkup.');
                console.error(await response.text());
            }
        } catch (error) {
            console.error('Error updating checkup:', error);
            alert('An unexpected error occurred.');
        }
    };

    // Cancel handler
    row.querySelector('.cancel-btn').onclick = () => {
        loadAllCheckups();
    };
}



async function deleteCheckup(checkupId) {
    const confirmDelete = confirm("Are you sure you want to delete this checkup?");

    if (!confirmDelete) return;

    try {
        const response = await fetch(`https://localhost:7023/api/checkups/${checkupId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("Checkup deleted successfully!");
            loadAllCheckups(); 
        } else {
            alert("Failed to delete checkup.");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while deleting checkup:", error);
    }
}



// =========================
// Image Section
// =========================

document.getElementById('imageUploadForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const checkupId = document.getElementById('checkupId').value;
    const imageFile = document.getElementById('imageFile').files[0]; 

    if (!imageFile) {
        alert("Please select a file.");
        return;
    }

    const formData = new FormData();
    formData.append("file", imageFile); 
    formData.append("checkupId", checkupId); 

    try {
        const response = await fetch('https://localhost:7023/api/images/upload', {
            method: 'POST',
            body: formData, 
        });

        if (response.ok) {
            alert("Image uploaded successfully!");
            document.getElementById('imageUploadForm').reset(); 
        } else {
            const errorText = await response.text();
            alert(`Failed to upload image: ${errorText}`);
            console.error(errorText);
        }
    } catch (error) {
        console.error("Error during upload:", error);
        alert("An unexpected error occurred.");
    }
});


document.getElementById('loadImages').addEventListener('click', async function () {
    try {
        console.log("Fetching images from /api/images...");
        const response = await fetch('https://localhost:7023/api/images');

        if (response.ok) {
            console.log("Images fetched successfully.");
            const images = await response.json();
            console.log("Fetched images:", images);

            const tableBody = document.getElementById('imagesTableBody');
            tableBody.innerHTML = '';

            if (images.length === 0) {
                tableBody.innerHTML = `<tr><td colspan="4">No images found in database</td></tr>`;
                return;
            }

            images.forEach(image => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${image.imageId}</td>
                    <td>${image.checkupId}</td>
                    <td>
                        <a href="${image.imageUrl}" target="_blank" class="image-link">
                            View Image
                        </a>
                    </td>
                    <td>
                        <img src="${image.imageUrl}" alt="Image preview" class="thumbnail">
                    </td>
<td>
            
            <button onclick="deleteImage(${image.imageId})">Delete</button>
        </td>
                `;
                tableBody.appendChild(row);
            });
        } else {
            const errorText = await response.text();
            console.error(`Failed to load images: ${response.status} ${response.statusText}`);
            throw new Error(`HTTP error! status: ${response.status}\n${errorText}`);
        }
    } catch (error) {
        console.error("Error loading images:", error);
        alert("Failed to load images. Check console for details.");
    }
});

async function deleteImage(imageId) {
    const confirmDelete = confirm("Are you sure you want to delete this image?");

    if (!confirmDelete) return;

    try {
        const response = await fetch(`https://localhost:7023/api/images/${imageId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("Image deleted successfully!");
            loadImages(); 
        } else {
            alert("Failed to delete image.");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while deleting image:", error);
    }
}

// =========================
// Export Section
// =========================


document.getElementById('exportButton').addEventListener('click', async function (e) {
    e.preventDefault();

    try {
        const response = await fetch('https://localhost:7023/api/patients/export', {
            method: 'GET',
        });

        if (response.ok) {
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = 'patients.csv';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        } else {
            alert('Failed to export patients data.');
            console.error(`Error: ${response.status} ${response.statusText}`);
        }
    } catch (error) {
        console.error('Error exporting data:', error);
        alert('An unexpected error occurred while exporting patients data.');
    }
});


// =========================
// Prescription Section
// =========================



document.getElementById('prescriptionForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    // Get values from form fields
    const checkupId = parseInt(document.getElementById('checkupId').value, 10);
    const medicationname = document.getElementById('medicationname').value;
    const dosage = document.getElementById('dosage').value;
    const startdate = document.getElementById('startdate').value;
    const enddate = document.getElementById('enddate').value || null;

    

    // Send POST request
    const response = await fetch('https://localhost:7023/api/prescriptions', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            checkupId,
            medicationname,
            dosage,
            startdate,
            enddate
        })
    });

    if (!startdate) {
        alert("Start Date is required.");
        return;
    }


    if (response.ok) {
        alert('Prescription added successfully!');
        document.getElementById('prescriptionForm').reset();
        loadPrescriptions(); 
    } else {
        alert('Failed to add prescription.');
        console.error(await response.text());
    }
});



document.getElementById('loadPrescriptions').addEventListener('click', loadPrescriptions);

async function loadPrescriptions() {
    try {
        const response = await fetch('https://localhost:7023/api/prescriptions');

        if (response.ok) {
            const prescriptions = await response.json();
            const tableBody = document.getElementById('prescriptionsTableBody');

            
            tableBody.innerHTML = '';

            if (prescriptions.length === 0) {
                tableBody.innerHTML = '<tr><td colspan="7">No prescriptions found.</td></tr>';
                return;
            }

            prescriptions.forEach(prescription => {
                const startdate = new Date(prescription.startdate).toLocaleDateString();
                const enddate = prescription.enddate ? new Date(prescription.enddate).toLocaleDateString() : 'N/A';

                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${prescription.prescriptionId}</td>
                    <td>${prescription.medicationname}</td>
                    <td>${prescription.dosage}</td>
                    <td>${startdate}</td>
                    <td>${enddate}</td>
                    <td>${prescription.checkupId}</td>
<td>  
<button class="edit-btn">Edit</button>
            <button onclick="deletePrescription(${prescription.prescriptionId})">Delete</button>
        </td>`;
                tableBody.appendChild(row);

                row.querySelector('button.edit-btn').addEventListener('click', () => {
                    enableInlineEditing(row, prescription);
                });
            });
        } else {
            const errorText = await response.text();
            alert(`Failed to load prescriptions: ${errorText}`);
            console.error(errorText);
        }
    } catch (error) {
        console.error('Error loading prescriptions:', error);
        alert('An unexpected error occurred.');
    }
}


async function enableInlineEditing(row, prescription) {
    // Replace each cell (except Prescription ID and Actions) with input fields
    row.innerHTML = `
        <td>${prescription.prescriptionId}</td>
        <td><input type="text" value="${prescription.medicationname}" class="edit-medicationname"></td>
        <td><input type="text" value="${prescription.dosage}" class="edit-dosage"></td>
        <td><input type="date" value="${prescription.startdate ? prescription.startdate.slice(0, 10) : ''}" class="edit-startdate"></td>
        <td><input type="date" value="${prescription.enddate ? prescription.enddate.slice(0, 10) : ''}" class="edit-enddate"></td>
        <td><input type="number" value="${prescription.checkupId}" class="edit-checkupid"></td>
        <td>
            <button class="save-btn">Save</button>
            <button class="cancel-btn">Cancel</button>
        </td>
    `;

    // Save logic
    row.querySelector('.save-btn').onclick = async function () {
        const updatedPrescription = {
            medicationname: row.querySelector('.edit-medicationname').value,
            dosage: row.querySelector('.edit-dosage').value,
            startdate: row.querySelector('.edit-startdate').value,
            enddate: row.querySelector('.edit-enddate').value || null,
            checkupId: parseInt(row.querySelector('.edit-checkupid').value, 10)
        };

        try {
            const response = await fetch(`https://localhost:7023/api/prescriptions/${prescription.prescriptionId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedPrescription)
            });

            if (response.ok) {
                alert('Prescription updated!');
                loadPrescriptions(); // Reload the table
            } else {
                const errorText = await response.text();
                alert(`Failed to update prescription: ${errorText}`);
            }
        } catch (error) {
            console.error('Error updating prescription:', error);
            alert('An unexpected error occurred.');
        }
    };

    // Cancel logic
    row.querySelector('.cancel-btn').onclick = function () {
        loadPrescriptions(); // Reload original data
    };
}





async function deletePrescription(prescriptionId) {
    const confirmDelete = confirm("Are you sure you want to delete this prescription?");

    if (!confirmDelete) return;

    try {
        const response = await fetch(`https://localhost:7023/api/prescriptions/${prescriptionId}`, {
            method: 'DELETE'
        });

        if (response.ok) {
            alert("Prescription deleted successfully!");
            loadImages();
        } else {
            alert("Failed to delete prescription.");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while deleting prescription:", error);
    }
}





