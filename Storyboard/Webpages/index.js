const express = require('express');
const app = express();
const port = 3000;

app.use(express.json());

app.get('/', (req, res) => {
    res.send('Hello World!');
    res.sendFile(path.join(__dirname, 'index.html'));
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});

const sql = require('mssql');

const config = {
    user: 'GRP-03-14',
    password: 'grp-03-14-2024#',
    server: 'soit-sql.mandela.ac.za',
    database: 'GRP-03-14',
    options: {
        encrypt: true,
        trustServerCertificate: true
    }
};

sql.connect(config).then(pool => {
    if (pool.connected){
        console.log('Connected to SQL Server');
    }
}).catch(err => {
    console.error('Database connection failed: ', err);
});

app.post('/add-user', async (req, res) => {
    console.log("Here");
    /*try {
        
        const pool = await sql.connect(config);
        const result = await pool.request()
            .input('UserID', sql.Int, 1)
            .input('FirstName', sql.nvarchar(50), "John")
            .input('LastName', sql.nvarchar(50), "Doe")
            .input('Password', sql.nvarchar(50), "12345")
            .input('Email', sql.nvarchar(50), "JD@email.com")
            .input('PhoneNumber', sql.nvarchar(50), "0123456789")
            .query('INSERT INTO User (UserID, FirstName, LastName, Password, Email, PhoneNumber) VALUES (@UserID, @FirstName, @LastName, @Password, @Email, @PhoneNumber)');
        console.log("Here2");
        console.log('User added');
    } catch (err) {
        console.log('Error inserting user: ' + err);
    }*/
});