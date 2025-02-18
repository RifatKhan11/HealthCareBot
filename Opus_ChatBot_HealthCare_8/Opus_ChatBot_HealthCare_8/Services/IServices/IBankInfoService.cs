using Opus_ChatBot_HealthCare_8.Models.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IBankInfoService
    {
        Task<bool> CheckBankAccount(string accountNumber);
        Task<BankAccountDetails> GetBankInformationByAccount(string accountNumber);
        Task<Remittance> GetRemittanceInfoByAcNumber(string accountNumber);
    }
}
