using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using VacuumPro.Api.Data;
using VacuumPro.Api.Models;

namespace VacuumPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacuumsController : ControllerBase
{
    private readonly VacuumDbContext _db;
    private readonly IWebHostEnvironment _env;

    public VacuumsController(VacuumDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // GET /api/vacuums
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VacuumDto>>> GetAll()
    {
        try
        {
            var vacuums = await _db.Vacuums
                .Select(v => new VacuumDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Picture = v.PicturePath,
                    Price = v.Price,
                    Power = v.Power,
                    Description = v.Description
                })
                .ToListAsync();

            return Ok(vacuums);
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    // GET /api/vacuums/1
    [HttpGet("{id}")]
    public async Task<ActionResult<VacuumDto>> GetById(int id)
    {
        try
        {
            var vacuum = await _db.Vacuums.FindAsync(id);

            if (vacuum is null)
                return NotFound(ApiErrorResponse.Create(404, $"Nie znaleziono odkurzacza o id {id}"));

            return Ok(new VacuumDto
            {
                Id = vacuum.Id,
                Name = vacuum.Name,
                Picture = vacuum.PicturePath,
                Price = vacuum.Price,
                Power = vacuum.Power,
                Description = vacuum.Description
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    // POST /api/vacuums
    [HttpPost]
    public async Task<ActionResult<VacuumDto>> Create([FromBody] VacuumCreateDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(ApiErrorResponse.Create(400, "Podaj nazwę produktu"));

            if (dto.Price <= 0)
                return BadRequest(ApiErrorResponse.Create(400, "Cena musi być większa od 0"));

            if (dto.Power < 0)
                return BadRequest(ApiErrorResponse.Create(400, "Moc nie może być ujemna"));

            var vacuum = new Vacuum
            {
                Name = dto.Name.Trim(),
                PicturePath = dto.Picture ?? string.Empty,
                Price = dto.Price,
                Power = dto.Power,
                Description = dto.Description ?? string.Empty
            };

            _db.Vacuums.Add(vacuum);
            await _db.SaveChangesAsync();

            var result = new VacuumDto
            {
                Id = vacuum.Id,
                Name = vacuum.Name,
                Picture = vacuum.PicturePath,
                Price = vacuum.Price,
                Power = vacuum.Power,
                Description = vacuum.Description
            };

            return CreatedAtAction(nameof(GetById), new { id = vacuum.Id }, result);
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VacuumDto>> Update(int id, [FromBody] VacuumCreateDto dto)
    {
        try
        {
            var vacuum = await _db.Vacuums.FindAsync(id);

            if (vacuum is null)
                return NotFound(ApiErrorResponse.Create(404, $"Nie znaleziono odkurzacza o id {id}"));

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(ApiErrorResponse.Create(400, "Podaj nazwę produktu"));

            if (dto.Price <= 0)
                return BadRequest(ApiErrorResponse.Create(400, "Cena musi być większa od 0"));

            if (dto.Power < 0)
                return BadRequest(ApiErrorResponse.Create(400, "Moc nie może być ujemna"));

            vacuum.Name = dto.Name.Trim();
            vacuum.PicturePath = dto.Picture ?? string.Empty;
            vacuum.Price = dto.Price;
            vacuum.Power = dto.Power;
            vacuum.Description = dto.Description ?? string.Empty;

            await _db.SaveChangesAsync();

            return Ok(new VacuumDto
            {
                Id = vacuum.Id,
                Name = vacuum.Name,
                Picture = vacuum.PicturePath,
                Price = vacuum.Price,
                Power = vacuum.Power,
                Description = vacuum.Description
            });
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var vacuum = await _db.Vacuums.FindAsync(id);

            if (vacuum is null)
                return NotFound(ApiErrorResponse.Create(404, $"Nie znaleziono odkurzacza o id {id}"));

            _db.Vacuums.Remove(vacuum);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    [HttpPost("{id}/image")]
    public async Task<ActionResult> UploadImage(int id, IFormFile file)
    {
        try
        {
            var vacuum = await _db.Vacuums.FindAsync(id);
            if (vacuum is null)
                return NotFound(ApiErrorResponse.Create(404, $"Nie znaleziono odkurzacza o id {id}"));

            if (file is null)
                return BadRequest(ApiErrorResponse.Create(400, "Brak pliku w żądaniu"));

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension is not ".jpg" and not ".jpeg" and not ".png")
                return BadRequest(ApiErrorResponse.Create(400, "Dozwolone rozszerzenia: .jpg, .jpeg, .png"));

            const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxSizeBytes)
                return BadRequest(ApiErrorResponse.Create(400, "Maksymalny rozmiar pliku to 5 MB"));

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{id}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            vacuum.PicturePath = $"/uploads/{fileName}";
            await _db.SaveChangesAsync();

            return Ok(new { picturePath = vacuum.PicturePath });
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    [HttpGet("{id}/image")]
    public async Task<ActionResult> GetImage(int id)
    {
        try
        {
            var vacuum = await _db.Vacuums.FindAsync(id);
            if (vacuum is null)
                return NotFound(ApiErrorResponse.Create(404, $"Nie znaleziono odkurzacza o id {id}"));

            if (string.IsNullOrWhiteSpace(vacuum.PicturePath))
                return NotFound(ApiErrorResponse.Create(404, "Obraz nie został przypisany"));

            var uploadPath = Path.Combine(_env.WebRootPath ?? "wwwroot", vacuum.PicturePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(uploadPath))
                return NotFound(ApiErrorResponse.Create(404, "Plik obrazu nie został znaleziony"));

            var contentType = GetContentType(uploadPath);
            return PhysicalFile(uploadPath, contentType);
        }
        catch (Exception)
        {
            return StatusCode(500, ApiErrorResponse.Create(500, "Wystąpił błąd serwera"));
        }
    }

    private static string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            _ => "application/octet-stream",
        };
    }
}
