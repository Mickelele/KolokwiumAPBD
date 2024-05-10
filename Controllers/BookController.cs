using Kolos.Models;
using Kolos.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolos.Controllers;


[ApiController]
[Route("api/books")]
public class BookController : ControllerBase
{
    private BookRepository _bookRepository;

    public BookController(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    
    
    [HttpGet("{id:int}/authors")]
    public async Task<IActionResult> getBookAuthors(int id)
    {

        var logiczna = await _bookRepository.czyIstniejeKsiazka(id);
        if (logiczna == false)
        {
            return NotFound("Podana ksiazka nieistnieje");
        }

        var obiekt = await _bookRepository.getBookAuthors(id);
        
        
        return Ok(obiekt);

    }




    [HttpPost]
    public async Task<IActionResult> insertBook(BookToInsert bookToInsert)
    {
        await _bookRepository.insertBook(bookToInsert);
        return Ok();
    }


}