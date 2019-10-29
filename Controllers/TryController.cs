using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP_Web_Reports.Models;

namespace ASP_Web_Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TryController : ControllerBase
    {
        private readonly SPContext _context;

        public TryController(SPContext context)
        {
            _context = context;
        }

        // GET: api/Try
        [HttpGet]
        public IEnumerable<TB_USER> GetTB_USER()
        {
            return _context.TB_USER;
        }

        // GET: api/Try/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTB_USER([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tB_USER = await _context.TB_USER.FindAsync(id);

            if (tB_USER == null)
            {
                return NotFound();
            }

            return Ok(tB_USER);
        }

        // PUT: api/Try/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTB_USER([FromRoute] string id, [FromBody] TB_USER tB_USER)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tB_USER.UserId)
            {
                return BadRequest();
            }

            _context.Entry(tB_USER).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TB_USERExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Try
        [HttpPost]
        public async Task<IActionResult> PostTB_USER([FromBody] TB_USER tB_USER)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TB_USER.Add(tB_USER);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTB_USER", new { id = tB_USER.UserId }, tB_USER);
        }

        // DELETE: api/Try/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTB_USER([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tB_USER = await _context.TB_USER.FindAsync(id);
            if (tB_USER == null)
            {
                return NotFound();
            }

            _context.TB_USER.Remove(tB_USER);
            await _context.SaveChangesAsync();

            return Ok(tB_USER);
        }

        private bool TB_USERExists(string id)
        {
            return _context.TB_USER.Any(e => e.UserId == id);
        }
    }
}