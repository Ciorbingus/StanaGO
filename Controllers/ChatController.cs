using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StanaGO.Data;
using StanaGO.Models;

namespace StanaGO.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly StanaGOContext _context; 
        private readonly UserManager<User> _userManager;

        public ChatController(StanaGOContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inbox()
        {
            var currentUserId = _userManager.GetUserId(User);

            var allMessages = await _context.PrivateMessages
                .Include(m => m.Sender).ThenInclude(u => u.UserProfile)
                .Include(m => m.Receiver).ThenInclude(u => u.UserProfile)
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .OrderByDescending(m => m.SentTime) 
                .ToListAsync();

            var inboxList = allMessages
                .GroupBy(m => m.SenderId == currentUserId ? m.Receiver : m.Sender)
                .Select(g => new InboxViewModel
                {
                    Partner = g.Key,
                    LastMessage = g.First().Content,
                    SentTime = g.First().SentTime,
                    UnreadCount = g.Count(m => m.ReceiverId == currentUserId && !m.IsRead)
                })
                .ToList();

            return View(inboxList);
        }


        [HttpGet]
        public async Task<IActionResult> Chat(string id) 
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Inbox");

            var currentUserId = _userManager.GetUserId(User);

            var partner = await _context.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (partner == null) return NotFound();

            var unreadMessages = await _context.PrivateMessages
                .Where(m => m.ReceiverId == currentUserId && m.SenderId == id && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(m => m.IsRead = true);
                await _context.SaveChangesAsync();
            }

            var messages = await _context.PrivateMessages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == id) ||
                            (m.SenderId == id && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentTime) 
                .ToListAsync();

            var viewModel = new ChatViewModel
            {
                PartnerId = partner.Id,
                PartnerName = $"{partner.FirstName} {partner.LastName}",
                PartnerImage = partner.UserProfile?.ImagePath,
                Messages = messages,
                CurrentUserId = currentUserId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(receiverId))
                return BadRequest();

            var msg = new PrivateMessage
            {
                SenderId = _userManager.GetUserId(User),
                ReceiverId = receiverId,
                Content = content,
                SentTime = DateTimeOffset.UtcNow, 
                IsRead = false
            };

            _context.PrivateMessages.Add(msg);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class InboxViewModel
    {
        public User Partner { get; set; }
        public string LastMessage { get; set; }
        public DateTimeOffset SentTime { get; set; }
        public int UnreadCount { get; set; }
    }

    public class ChatViewModel
    {
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerImage { get; set; }
        public List<PrivateMessage> Messages { get; set; }
        public string CurrentUserId { get; set; }
    }
}