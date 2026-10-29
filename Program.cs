
using LibraryManagement.Services;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LibraryManagement.Repository;

string connectionString =
    @"Server=.\SQLEXPRESS;Database=StudentCourseDB;Trusted_Connection=True;TrustServerCertificate=True;";

var BookRepository = new BookRepository(connectionString);

var BookService = new BookService(BookRepository);

