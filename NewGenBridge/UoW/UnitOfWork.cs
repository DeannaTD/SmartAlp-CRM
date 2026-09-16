using NewGenBridge.Data;
using NewGenBridge.Repositories;

namespace NewGenBridge.UoW
{
    public class UnitOfWork
    {
        ApplicationDbContext _context;
        public AttendanceRepository Attendances { get; set; }
        public DivisionRepository Divisions { get; set; }
        public BranchRepository Branches { get; set; }
        public GroupRepository Groups { get; set; }
        public LessonRepository Lessons { get; set; }
        public LevelRepository Levels { get; set; }
        public PaymentPeriodRepository PaymentPeriods { get; set; }
        public PaymentRepository Payments { get; set; }
        public StatusRepository Statuses { get; set; }
        public UserLessonRepository UserLessons { get; set; }
        public UserRepository Users { get; set; }
        public UserAccessRepository UserAccesses { get; set; }
        public PriceRepository Prices { get; set; }
        public BillRepository Bills { get; set; }
        public ContractRepository Contracts { get; set; }
        public MentorLevelRepository MentorLevels { get; set; }
        public HubFileRepository HubFiles { get; set; }
        public HubFileRoleRepository HubFileRoles { get; set; }
        public TranslationRepository Translations { get; set; }
        public LeadRepository Leads { get; set; }
        public CompanyRepository Companies { get; set; }
        public KitRequestRepository KitRequests { get; set; }
        public AgreementRepository Agreements { get; set; }
        public RoyaltyRepository Royalties { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Divisions = new DivisionRepository(_context);
            Attendances = new AttendanceRepository(_context);
            Branches = new BranchRepository(_context);
            Groups = new GroupRepository(_context);
            Lessons = new LessonRepository(_context);
            Levels = new LevelRepository(_context);
            PaymentPeriods = new PaymentPeriodRepository(_context);
            Payments = new PaymentRepository(_context);
            Statuses = new StatusRepository(_context);
            UserLessons = new UserLessonRepository(_context);
            Users = new UserRepository(_context);
            UserAccesses = new UserAccessRepository(_context);
            Prices = new PriceRepository(_context);
            Bills = new BillRepository(_context);
            Contracts = new ContractRepository(_context);
            MentorLevels = new MentorLevelRepository(_context);
            HubFiles = new HubFileRepository(_context);
            HubFileRoles = new HubFileRoleRepository(_context);
            Translations = new TranslationRepository(_context);
            Leads = new LeadRepository(_context);
            Companies = new CompanyRepository(_context);
            KitRequests = new KitRequestRepository(_context);
            Agreements = new AgreementRepository(_context);
            Royalties = new RoyaltyRepository(_context);
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
