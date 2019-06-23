using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BKBSports.Models
{
    public class ArticleModel
    {
        //-- Article Id --//
        [Key]
        public int articleId { get; set; }

        //-- Creation Timestamp --//
        [Display(Name = "Written on: ")]
        public DateTime articleCreateDate { get; set; }

        //-- Updated Timestamp --//
        [Display(Name = "Last Update: ")]
        public DateTime articleUpdateTimestamp { get; set; }

        //-- Author Id --//
        [Display(Name = "Author")]
        public int authorId { get; set; }
        public virtual UserProfile Author { get; set; }

        //-- Layout Design --//
        [Display(Name = "Which Layout would you like")]
        [Required(ErrorMessage ="You must select a layout")]
        public Layout layout { get; set; }

        //-- Approval Indicator --//
        [Display(Name = "Approved")]
        public Approval approvalIndicator { get; set; }

        //-- Article Image --//
        public byte[] articleImage { get; set; }

        //-- Artilce Content --//
        [AllowHtml]
        [Display(Name = "Article")]
        [Required(ErrorMessage = "There must be content")]
        public string articleContent { get; set; }
    }

    public enum Layout
    {
        one = 1,
        two = 2,
        three = 3,
        four =4
    }

    public enum Approval
    {
        YES = 1,
        NO = 2
    }
}