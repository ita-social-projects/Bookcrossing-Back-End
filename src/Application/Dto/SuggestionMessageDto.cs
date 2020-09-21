using Domain.RDBMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto
{
    public class SuggestionMessageDto
    {
        public int Id { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public MessageState State { get; set; }
        public int UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
    }
}
