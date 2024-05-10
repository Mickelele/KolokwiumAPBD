using System.Data.SqlClient;
using Kolos.Models;

namespace Kolos.Repositories;

public class BookRepository
{
    private string? _connectionString;

    public BookRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }


    public async Task<AutorKsiazki> getBookAuthors(int id)
    {

        string query =
            "SELECT books.PK, books.title, authors.first_name, authors.last_name FROM books JOIN books_authors ON books_authors.FK_book = books.PK JOIN authors ON authors.PK = books_authors.FK_author WHERE books.PK = @ID";
        
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@ID", id);
        
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        AutorKsiazki result = null;
        
        
        
        while (await reader.ReadAsync())
        {
            if (result is not null)
            {
                result.autorzy.Add(new Autor()
                {
                    firstName = reader.GetString(2),
                    lastName = reader.GetString(3)
                });
            }
            else
            {
                result = new AutorKsiazki()
                {
                    id = reader.GetInt32(0),
                    title = reader.GetString(1),
                    autorzy = new List<Autor>()
                    {
                        new Autor()
                        {
                            firstName = reader.GetString(2),
                            lastName = reader.GetString(3)
                        }
                    }
                };
                
            }
        }
        
        if (result is null) throw new Exception();


        return result;

    }

    public async Task<bool> czyIstniejeKsiazka(int id)
    {

        string query = "SELECT COUNT(*) FROM books WHERE books.PK = @id";
        
        
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await connection.OpenAsync();
        
        var licznik = (int)await command.ExecuteScalarAsync();

        return licznik > 0;
    }
    
    
    
    public async Task insertBook(BookToInsert bookToInsert)
    {

        string query = @"INSERT INTO books VALUES (@title); SELECT SCOPE_IDENTITY();";
        
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand(query, connection);
        
        
        command.Parameters.AddWithValue("@title", bookToInsert.title);
        
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        int idKsiazki = 0;
        int idAutora = 0;
        
        try
        {
            idKsiazki = Convert.ToInt32(await command.ExecuteScalarAsync());
    
            foreach (var autor in bookToInsert.autorzy)
            {
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO authors VALUES(@firstName, @lastName);";
                command.CommandText = "INSERT INTO authors VALUES(@firstName, @lastName); SELECT SCOPE_IDENTITY();";
                command.Parameters.AddWithValue("@firstName", autor.firstName);
                command.Parameters.AddWithValue("@lastName", autor.lastName);
                
                idAutora = Convert.ToInt32(await command.ExecuteScalarAsync());
                
                command.Parameters.AddWithValue("@idAutora", idAutora);
                command.Parameters.AddWithValue("@idKsiazki", idKsiazki);
                
                command.CommandText = "INSERT INTO books_authors VALUES (@idKsiazki, @idAutora);";

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
    
    
    
    
    
    
    
    
    
    

}