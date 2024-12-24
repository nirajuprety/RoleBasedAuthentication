# Role-Based Authentication Implementation using ASP.NET Identity

This project demonstrates role-based authentication with four levels using ASP.NET Identity. The application implements authentication for the following roles:

- **Super Admin**
- **Staff**
- **Low-Level Staff**
- **User**

Each role is associated with a predefined user and password for testing purposes.

## Roles and Users

### Defined Roles:
- **Super Admin**
- **Staff**
- **Low-Level Staff**
- **User**

### Predefined Users:

- **Super Admin**:  
  - **Email**: `superadmin@example.com`  
  - **Password**: `SuperAdmin@123`  
  - **Role**: `Super Admin`

- **Staff**:  
  - **Email**: `staff@example.com`  
  - **Password**: `Staff@123`  
  - **Role**: `Staff`

- **Low-Level Staff**:  
  - **Email**: `lowlevelstaff@example.com`  
  - **Password**: `LowLevelStaff@123`  
  - **Role**: `Low-Level Staff`

- **User**:  
  - **Email**: `user@example.com`  
  - **Password**: `User@123`  
  - **Role**: `User`

## API Endpoints

### 1. **Login API**

#### URL: 
`https://localhost:44392/api/AuthApi/login`

#### Method:
`POST`

#### Request Body:
```json
{
  "email": "user@example.com",
  "password": "User@123"
}
```
#### Request Body:
```json
{
  "email": "user@example.com",
  "password": "User@123"
}
```
### 2. **Access Protected API with Token**
Once you have obtained the JWT token from the login API, you can use the token to authenticate and access other protected APIs.
#### Request Header:
```json
Authorization: bearer <Your_JWT_Token_Here>
```
Example 
```json
bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidXNlckBleGFtcGxlLmNvbSIsImp0aSI6ImRjMTE2NzBiLWM0YWMtNGRkNi04MGMxLTNkNWM5MTA4ZThmYyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3MzUwMjE3MTgsImlzcyI6IkpXVEF1dGhlbnRpY2F0aW9uU2VydmVyIiwiYXVkIjoiSldUU2VydmljZVBvc3RtYW5DbGllbnQifQ.k4xnDOVOSkVxe8hN6_iymgCXqjqRteBenG9jAvj2h7g
```

### 3. **Steps to Test**
Steps to Test
Login using the provided email and password for any predefined user by sending a POST request to https://localhost:44392/api/AuthApi/login.

Obtain the JWT Token from the response and copy the token value.

Access Protected API by including the token in the Authorization header as a Bearer token in subsequent requests.

### 4. **Note**

Ensure that the appsettings.json is properly configured with your database connection for authentication.
The JWT Token is required to access any API that requires role-based authorization.
The four roles defined in this project (Super Admin, Staff, Low-Level Staff, and User) are statically assigned to users.
The APIs are protected by roles, and users with appropriate roles can access the respective resources.
