using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Models;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using Opus_ChatBot_HealthCare_8.Models.ApiModelData;
using Opus_ChatBot_HealthCare_8.Models.ApiModels;
using Opus_ChatBot_HealthCare_8.Models.ApiViewModels;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.BotViewModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using Opus_ChatBot_HealthCare_8.Models.SupportModels;


namespace Opus_ChatBot_HealthCare_8.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


//------------------------------------------------------------------------------------------------------
        #region Mapped Entity

        public DbSet<FacebookPage> FacebookPages { get; set; } // Used to store page Related infos 
        public DbSet<Menu> Menus { get; set; } // All Related Menus.
        public DbSet<MessageLog> MessageLogs { get; set; }
        public DbSet<MenuReader> MenuReaders { get; set; }
        public DbSet<Question> Questions { get; set; } // Contains Question For a menu
        public DbSet<AnswerType> AnswerTypes { get; set; } // Used For Response Type Indication.
        public DbSet<Answer> Answers { get; set; } // Contains Related Answer For a Question. 
        public DbSet<Queries> Queries { get; set; } //Contains Users Custom queris.
        public DbSet<Analytics> Analytics { get; set; } //All Related Data Used To generate Analytics.
        public DbSet<Language> Languages { get; set; } // Contains User selected Language Info.
        public DbSet<Models.BotModels.File> Files { get; set; } // Contains Uploaded File Info.
        public DbSet<BotFlow> BotFlows { get; set; } //Contains Bot Fow Info.
        public DbSet<ServiceFlow> ServiceFlows { get; set; } // Contains Custom service flow info.
        public DbSet<UserInfo> UserInfos { get; set; } // Contains User infos For a Bot.
        public DbSet<BotRackInfoMaster> botRackInfoMasters { get; set; } 
        public DbSet<BotRackInfoDetail> botRackInfoDetails { get; set; } 
        public DbSet<CardGroupMaster> CardGroupMasters { get; set; } 
        public DbSet<CardGroupDetail> CardGroupDetails { get; set; } 
        public DbSet<QuestionNavigation> QuestionNavigations { get; set; } 
        public DbSet<OTPCode> OTPCodes { get; set; } 
        public DbSet<BotKnowledge> BotKnowledges { get; set; } 
        public DbSet<DepartmentInfo> DepartmentInfos { get; set; } 
        public DbSet<Models.BotModels.ConnectionInfo> ConnectionInfos { get; set; } 

        public DbSet<PassportInfo> passportInfos { get; set; } //Contains Demo Data For POLICE HEAD OFFICE Service
        
        public DbSet<BankAccountDetails> bankAccountDetails { get; set; } // For Demo Bank Info Service Reserve Test data.
        public DbSet<Remittance> remittances { get; set; } // For Demo Bank Info Service Reserve Test data.
        public DbSet<KeyWordQuesAns> keyWordQuesAns { get; set; }
        public DbSet<Operator> operators { get; set; }
        public DbSet<LastGrettings> lastGrettings { get; set; }
        public DbSet<OperatorAssign> operatorAssigns { get; set; }
        public DbSet<ComplainSuggestion> complainSuggestions { get; set; }
        public DbSet<KeyWordInfo> KeyWordInfos { get; set; }
        public DbSet<WrapperMessage> WrapperMessages { get; set; }
        public DbSet<InputGroupMaster> InputGroupMasters { get; set; }
        public DbSet<InputGroupDetail> InputGroupDetails { get; set; }
        public DbSet<UserFeedback> UserFeedbacks { get; set; }
        public DbSet<UserQuery> UserQueries { get; set; }

        public DbSet<PoliceClearenceLog> policeClearenceLogs { get; set; }//Contains User Log who checked police clearence Data
        public DbSet<MenuHitLog> menuHitLogs { get; set; }//menu hit log
        public DbSet<KnowledgeHitLog> knowledgeHitLogs { get; set; }//KeyWordQuesAns hit log
        public DbSet<UnKnownKeyWordQuestion> unKnownKeyWordQuestions { get; set; }
        public DbSet<DoctorInfo> DoctorInfos { get; set; }
        public DbSet<AppoinmentInfo> AppoinmentInfos { get; set; }
        public DbSet<DoctorVisitTimePeriod> DoctorVisitTimePeriods { get; set; }        
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }        
        public DbSet<BranchInfo> BranchInfos { get; set; }        
        public DbSet<WrapperHeaderImg> WrapperHeaderImgs { get; set; }        
        public DbSet<WrapperHeader> WrapperHeaders { get; set; }

        #endregion


        #region ApiModels
        public DbSet<ApiDepartment> ApiDepartments { get; set; }
        public DbSet<ApiDoctor> ApiDoctors { get; set; }
        public DbSet<ApiSpecialisation> ApiSpecialisation { get; set; }
        public DbSet<ApiDoctorSlot> ApiDoctorSlots { get; set; }


        public DbSet<EvercareToken> EvercareTokens { get; set; }
        public DbSet<ApiProcessLogInfo> ApiProcessLogs { get; set; }
        public DbSet<DepartmentLog> DepartmentLogs { get; set; }
        public DbSet<SpecializationLog> SpecializationLog { get; set; }
        public DbSet<DoctorLog> DoctorLogs { get; set; }
         public DbSet<ApiDepartmentData> ApiDepartmentData { get; set; }
        public DbSet<ApiDoctorData> ApiDoctorData { get; set; }
        public DbSet<ApiSpecialisationData> ApiSpecialisationData { get; set; }
        public DbSet<ApiActivityLog> ApiActivityLogs { get; set; }
        #endregion

        #region masterdata
        public DbSet<questionCategory> questionCategories { get; set; }
        public DbSet<Weeks> Weeks { get; set; }
        public DbSet<TimePeriod> TimePeriods { get; set; }
        public DbSet<ChatbotInfo> ChatbotInfos { get; set; }
        public DbSet<WebHookLog> WebHookLogs { get; set; }
        #endregion
        //-----------------------------------------------------------------------------------------------------
        #region Query Entity

        //public DbQuery<MenuQuestionAnswer> menuQuestionAnswers { get; set; } // Data Comes From SP
        //public DbQuery<TotalCountViewModel> totalCountViewModels { get; set; }
        //public DbQuery<TotalHitInfotViewModel> totalHitInfotViewModels { get; set; }
        //public DbQuery<TotalHitMenuLogViewModel> totalHitMenuLogViewModels { get; set; }
        //public DbQuery<TotalHitKnowledgeLogViewModel> totalHitKnowledgeLogViewModels { get; set; }
        //public DbQuery<QueriesDataViewModel> queriesDataViewModels { get; set; }
        //public DbQuery<DashboardListVM> DashboardListVMSPModels { get; set; }
        //public DbQuery<AppointmentVM> AppointmentVMs { get; set; }
        //public DbQuery<GetTimeSlotsByDoctorIdVm> GetTimeSlotsByDoctorIdVms { get; set; }
        //public DbQuery<UserFeedbackQueryViewModel> UserFeedbackQueryViewModels { get; set; }
        //public DbQuery<DoctorViewModel> DoctorVMs { get; set; }
        //public DbQuery<ApiMasterDataVM> apiMasterDataVMs { get; set; }
        //public DbQuery<ApiListVM> apiListVMs { get; set; }
        //public DbQuery<ColumnHeading> columnHeadings { get; set; }//Contains column name of passportinfos


        #endregion
        //------------------------------------------------------------------------------------------------------
        #region Bot MassegeHistory
        public DbSet<BotMessageHistory> botMessageHistories { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            #region Data Seed
            builder.Entity<AnswerType>().HasData(new AnswerType { Id = 1, Type = "Text" });
            builder.Entity<AnswerType>().HasData(new AnswerType { Id = 2, Type = "Text With Buttons" });
            builder.Entity<AnswerType>().HasData(new AnswerType { Id = 3, Type = "Carousel" });
            #endregion

            //Making page ID Unique
            builder.Entity<FacebookPage>(entity =>
            {
                entity.HasIndex(e => e.PageId).IsUnique(true);
            });

            builder.Entity<FacebookPage>()
            .HasOne(p => p.ApplicationUser)
            .WithOne(b => b.FacebookPage);

        }
    }
}
