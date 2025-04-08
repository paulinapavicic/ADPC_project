// =========================
// Patients Section
// =========================

// Add event listener to the patient form
document.getElementById('patientForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    // Get form data
    
    const personalIdentificationNumber = document.getElementById('personalIdentificationNumber').value;
    const name = document.getElementById('name').value;
    const surname = document.getElementById('surname').value;
    const dateOfBirth = document.getElementById('dateOfBirth').value;
    const sex = document.getElementById('sex').value;

    // Send POST request to the API
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
        loadPatients(); // Reload patients list
    } else {
        alert('Failed to add patient.');
        console.error(await response.text());
    }
});

// Load patients from the API
document.getElementById('loadPatients').addEventListener('click', loadPatients);

async function loadPatients() {
    const response = await fetch('https://localhost:7023/api/patients');

    if (response.ok) {
        const patients = await response.json();
        const tableBody = document.getElementById('patientsTableBody');

        // Clear existing rows
        tableBody.innerHTML = '';

        // Populate table with patients
        patients.forEach(patient => {
            // Format Date of Birth to YYYY-MM-DD
            const formattedDate = new Date(patient.dateOfBirth).toLocaleDateString('en-GB'); // Formats as DD/MM/YYYY


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

async function editPatient(patientId) {
    // Prompt user for new data
    const newName = prompt("Enter new name:");
    const newSurname = prompt("Enter new surname:");
    const newDateOfBirth = prompt("Enter new date of birth (YYYY-MM-DD):");
    const newSex = prompt("Enter new sex (M/F):");

    if (!newName || !newSurname || !newDateOfBirth || !newSex) {
        alert("All fields are required.");
        return;
    }

    try {
        // Send PUT request to update patient
        const response = await fetch(`https://localhost:7023/api/patients/${patientId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                name: newName,
                surname: newSurname,
                dateOfBirth: newDateOfBirth,
                sex: newSex
            })
        });

        if (response.ok) {
            alert("Patient updated successfully!");
            loadPatients(); // Reload patients table
        } else {
            alert("Failed to update patient.");
            console.error(await response.text());
        }
    } catch (error) {
        console.error("Error occurred while updating patient:", error);
    }
}



async function deletePatient(patientId) {
    const confirmDelete = confirm("Are you sure you want to delete this patient?");

    if (!confirmDelete) return;

    const response = await fetch(`https://localhost:7023/api/patients/${patientId}`, {
        method: 'DELETE'
    });

    if (response.ok) {
        alert("Patient deleted successfully!");
        loadPatients(); // Reload the table
    } else {
        alert("Failed to delete patient.");
    }
}



// =========================
// Medical Records Section
// =========================

// Add event listener to the medical record form
document.getElementById('medicalRecordForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    // Get form data
    const patientId = document.getElementById('patientId').value;
    const diseaseName = document.getElementById('diseaseName').value;
    const startDate = document.getElementById('startDate').value;
    const endDate = document.getElementById('endDate').value;

    // Send POST request to the API
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

// Load medical records from the API
document.getElementById('loadAllMedicalRecords').addEventListener('click', loadAllMedicalRecords);

async function loadAllMedicalRecords() {
    console.log("Fetching all medical records..."); // Debugging log

    try {
        // Fetch all medical records from the API
        const response = await fetch('https://localhost:7023/api/medicalrecords'); // Correct endpoint for all records

        if (response.ok) {
            const medicalRecords = await response.json(); // Parse JSON response
            console.log("Fetched all medical records:", medicalRecords); // Debugging log

            const tableBody = document.getElementById('medicalRecordsTableBody'); // Get table body element

            // Clear existing rows
            tableBody.innerHTML = '';

            // Populate table with medical records
            medicalRecords.forEach(record => {
                console.log("Adding record to table:", record); // Debugging log

               

                

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
            loadAllMedicalRecords(); // Reload the medical records table
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

// Add event listener to the checkup form
document.getElementById('checkupForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    // Get form data
    const patientId = document.getElementById('checkupPatientId').value;
    const procedureCode = document.getElementById('procedureCode').value;
    const checkupDate = document.getElementById('checkupDate').value;

    // Send POST request to the API
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
    console.log("Fetching all checkups..."); // Debugging log

    try {
        // Fetch all checkups from the API
        const response = await fetch('https://localhost:7023/api/checkups'); // Correct endpoint for all records

        if (response.ok) {
            const checkups = await response.json(); // Parse JSON response
            console.log("Fetched all checkups:", checkups); // Debugging log

            const tableBody = document.getElementById('checkupsTableBody'); // Get table body element

            // Clear existing rows
            tableBody.innerHTML = '';

            // Populate table with checkups
            checkups.forEach(checkup => {
                console.log("Adding record to table:", checkup); // Debugging log

                // Format Check-Up Date to a readable format
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
        </td>`; // Display formatted date and time
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
            loadAllCheckups(); // Reload the checkups table
        } else {
            alert("Failed to delete checkup.");
            console.error("Error response:", await response.text());
        }
    } catch (error) {
        console.error("Error occurred while deleting checkup:", error);
    }
}




// ========================= Images Section
document.getElementById('imageUploadForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const checkupId = document.getElementById('checkupId').value;
    const imageFile = document.getElementById('imageFile').files[0]; // Get the selected file

    if (!imageFile) {
        alert("Please select a file.");
        return;
    }

    const formData = new FormData();
    formData.append("file", imageFile); // Append the file
    formData.append("checkupId", checkupId); // Append the checkup ID

    try {
        const response = await fetch('https://localhost:7023/api/images/upload', {
            method: 'POST',
            body: formData, // Send the form data
        });

        if (response.ok) {
            alert("Image uploaded successfully!");
            document.getElementById('imageUploadForm').reset(); // Reset the form
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


// Function to load and display images
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











