
using Microsoft.EntityFrameworkCore;
using NewGenBridge.Data;
using NewGenBridge.Models;

namespace NewGenBridge.Services
{
    public class MidResetService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<MidResetService> _logger;

        public MidResetService(IServiceScopeFactory serviceScopeFactory, ILogger<MidResetService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MidServiceEx started");
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    List<Branch> branches = await db.Branches.ToListAsync(stoppingToken);
                    
                    foreach(Branch branch in branches)
                    {
                        if(string.IsNullOrEmpty(branch.TimeZoneId)) continue;

                        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(branch.TimeZoneId);
                        DateTime bt = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);

                        if(bt.Hour == 0 && bt.Minute < 10)
                        {
                            await db.Database.ExecuteSqlRawAsync(@"
                                UPDATE UserLessons
                                SET IsOpened = 0
                                WHERE UserId IN (
                                    SELECT u.Id
                                    FROM AspNetUsers u
                                    WHERE u.MentorLastGroupBranchId = {0}
                                ) AND IsOpened = 1
                                  AND CloseAt IS NOT NULL
                                  AND CAST(CloseAt AS DATE) <= {1}
                            ", branch.Id, bt.Date);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MidResetService error");
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
