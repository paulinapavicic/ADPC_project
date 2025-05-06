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
                    <button onclick="editPatient(${patient.patientId})">Edit</button>
                    <button onclick="deletePatient(${patient.patientId})">Delete</button>
                </td>`; 
            tableBody.appendChild(row);
        });

    } else {
        alert("Failed to load patients");
    }
}

document.querySelectorAll('.edit-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const row = btn.closest('tr');
        const patientId = row.dataset.patientId;
        row.innerHTML = `
      <td><input value="${row.querySelector('.name').innerText}" class="edit-name"></td>
      <td><input value="${row.querySelector('.surname').innerText}" class="edit-surname"></td>
      <td><input value="${row.querySelector('.dob').innerText}" class="edit-dob"></td>
      <td><input value="${row.querySelector('.sex').innerText}" class="edit-sex"></td>
      <td>
        <button class="save-btn">Save</button>
        <button class="cancel-btn">Cancel</button>
      </td>
    `;

        row.querySelector('.save-btn').onclick = async function () {
            const updatedPatient = {
                name: row.querySelector('.edit-name').value,
                surname: row.querySelector('.edit-surname').value,
                dateOfBirth: row.querySelector('.edit-dob').value,
                sex: row.querySelector('.edit-sex').value
            };
            const response = await fetch(`https://localhost:7023/api/patients/${patientId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(updatedPatient)
            });
            if (response.ok) {
                alert("Patient updated!");
                // Optionally reload or update row display
            } else {
                alert("Failed to update patient.");
            }
        };

        row.querySelector('.cancel-btn').onclick = function () {
            // Reload the table or restore the original row
            loadPatients();
        };
    });
});



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
                    <td>${record.patientId}</td>
                    <td>${record.diseaseName}</td>
                    <td>${record.startDate}</td>
                    <td>${record.endDate}</td>
<td> <button onclick="editMedicalRecord(${record.medicalRecordId})">Edit</button>
            <button onclick="deleteMedicalRecord(${record.medicalRecordId})">Delete</button>
        </td>`; 
                tableBody.appendChild(row);
            });
        } else {
            alert("Failed to load medical records");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while fetching medical records:", error);
    }
}


        

async function deleteMedicalRecord(medicalRecordId) {
    const confirmDelete = confirm("Are you sure you want to delete this medical record?");

    if (!confirmDelete) return;

    try {
        const response = await fetch(`https://localhost:7023/api/medicalrecords/${medicalRecordId}`, {
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
            <button onclick="editCheckup(${checkup.checkupId})">Edit</button>
            <button onclick="deleteCheckup(${checkup.checkupId})">Delete</button>
        </td>`; 
                tableBody.appendChild(row);
            });
        } else {
            alert("Failed to load checkups");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while fetching checkups:", error);
    }
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


document.querySelector('button').addEventListener('click', async function (e) {
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

    const data = {
        checkupId: parseInt(document.getElementById('checkupId').value),
        medicationName: document.getElementById('medicationName').value,
        dosage: document.getElementById('dosage').value,
        startDate: document.getElementById('startDate').value,
        endDate: document.getElementById('endDate').value || null
    };

    try {
        const response = await fetch('https://localhost:7023/api/prescriptions', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            alert('Prescription added successfully!');
            document.getElementById('prescriptionForm').reset(); 
            loadPrescriptions(); 
        } else {
            const errorText = await response.text();
            alert(`Failed to add prescription: ${errorText}`);
            console.error(errorText);
        }
    } catch (error) {
        console.error('Error adding prescription:', error);
        alert('An unexpected error occurred.');
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
                tableBody.innerHTML = '<tr><td colspan="6">No prescriptions found.</td></tr>';
                return;
            }

            prescriptions.forEach(prescription => {
                const startDate = new Date(prescription.startDate).toLocaleDateString();
                const endDate = prescription.endDate ? new Date(prescription.endDate).toLocaleDateString() : 'N/A';

                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${prescription.prescriptionId}</td>
                    <td>${prescription.medicationName}</td>
                    <td>${prescription.dosage}</td>
                    <td>${startDate}</td>
                    <td>${endDate}</td>
                    <td>${prescription.checkupId}</td>
<td> <button onclick="editPrescription(${prescription.prescriptionId})">Edit</button>
            <button onclick="deletePrescription(${prescription.prescriptionId})">Delete</button>
        </td>
                `;
                tableBody.appendChild(row);
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





