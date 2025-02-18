using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class BankInfoService : IBankInfoService
    {
        private readonly ApplicationDbContext _contex;

        public BankInfoService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> CheckBankAccount(string accountNumber)
        {
            BankAccountDetails bankAccountDetails =  await _contex.bankAccountDetails.Where(x => x.acNumber == accountNumber).AsNoTracking().FirstOrDefaultAsync();

            if (bankAccountDetails != null) return true;
            return false;
        }

        public async Task<BankAccountDetails> GetBankInformationByAccount(string accountNumber)
        {
            return await _contex.bankAccountDetails.Where(x => x.acNumber == accountNumber).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<Remittance> GetRemittanceInfoByAcNumber(string accountNumber)
        {
            return await _contex.remittances.Where(x => x.bankAccountDetails.acNumber == accountNumber).FirstOrDefaultAsync();
        }
    }
}
