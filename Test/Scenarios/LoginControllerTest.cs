using Xunit;
using System.Threading.Tasks;
using TaApostado.DTOs.InputModels;
using TaApostado.DTOs.OutputModels;
using System.Net.Http.Json;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Test.Utils;
using System;

namespace Test.Scenarios
{
    [TestCaseOrderer("Test.Utils.PriorityOrderer", "Test")]
    public class LoginControllerTest
    {
        private readonly TestContext _testContext;
        private readonly JsonSerializerOptions _options;
        public LoginControllerTest()
        {
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreReadOnlyProperties = true };
            _testContext = new TestContext();
        }

        [Fact, TestPriority(0)]
        public async Task SignUp_ReturnsOkResponse()
        {
            var body = new InputModelUser() { name = "Guilherme", lastName = "Rocha", email = "gui@gmail.com", password = "Teste123", cpf = "661.345.778-71"};

            var content = JsonContent.Create<InputModelUser>(body);

            var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

            var obj = await response.Content.ReadFromJsonAsync<OutputModelLogin>(_options);

            Console.WriteLine(obj);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(obj);
            Assert.True(obj is OutputModelLogin);
            Assert.True(!string.IsNullOrEmpty(obj.token));
            Assert.Empty(obj.user.challenges);
            Assert.Empty(obj.user.bets);
            Assert.Equal(body.name, obj.user.name);
            Assert.Equal(1000, obj.user.amount);   
        }

        [Fact, TestPriority(2)]
        public async Task SignUp_Name_Invalid_Post_ReturnsBadRequest()
        {
            var invalidNames = new List<string>() { "G", "Gu", "Guuussttavooooooo", "Gustavo1", "1234345321" };

            foreach (var invalid in invalidNames)
            {
                var body = new InputModelUser() { name = invalid, lastName = "Rocha", email = "gui@gmail.com", password = "Teste123", cpf = "661.345.778-71" };

                var content = JsonContent.Create<InputModelUser>(body);

                var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

                var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.True(400 == (int) obj.SelectToken("status"));
                Assert.Equal("The field name is invalid.", (string) obj.SelectToken("errors.name[0]"));
            }   
        }

        [Fact, TestPriority(2)]
        public async Task SignUp_LastName_Invalid_ReturnsBadRequest()
        {
            var invalidLastNames = new List<string>() { "S", "S", "GGGuuussstttavooooooo", "Silva1", "1234345321" };

            foreach (var invalid in invalidLastNames)
            {
                var body = new InputModelUser() { name = "Guilherme", lastName = invalid, email = "gui@gmail.com", password = "Teste123", cpf = "661.345.778-71" };

                var content = JsonContent.Create<InputModelUser>(body);

                var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

                var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.True(400 == (int)obj.SelectToken("status"));
                Assert.Equal("The field last name is invalid.", (string)obj.SelectToken("errors.lastName[0]"));
            }
        }

        [Fact, TestPriority(2)]
        public async Task SignUp_CPF_Invalid_ReturnsBadRequest()
        {
           var invalidCPFs = new List<string>() { "279.599.810-A0", "27959981060..-", "27959981060"};

            foreach (var invalid in invalidCPFs)
            {
                var body = new InputModelUser() { name = "Guilherme", lastName = "Rocha", email = "gui@gmail.com", password = "Teste123", cpf = invalid };

                var content = JsonContent.Create<InputModelUser>(body);

                var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

                var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.True(400 == (int)obj.SelectToken("status"));
                Assert.Equal("The field cpf is invalid.", (string)obj.SelectToken("errors.cpf[0]"));
            }
        }

        [Fact, TestPriority(2)]
        public async Task SignUp_Email_Invalid_ReturnsBadRequest()
        {
            var invalidEmails = new List<string>() { "@gmail.com", "g.com", "gustavo.com.br", "gustavo@gmail" };

            foreach (var invalid in invalidEmails)
            {
                var body = new InputModelUser() { name = "Guilherme", lastName = "Rocha", email = invalid, password = "Teste123", cpf = "661.345.778-71" };

                var content = JsonContent.Create<InputModelUser>(body);

                var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

                var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.True(400 == (int)obj.SelectToken("status"));
                Assert.Equal("The field email is invalid.", (string)obj.SelectToken("errors.email[0]"));
            }
        }

        [Fact, TestPriority(2)]
        public async Task SignUp_Password_Invalid_ReturnsBadRequest()
        {
            var invalidPasswords = new List<string>() { "Teste", "Teste1", "TesteTeste", "12345678" };

            foreach (var invalid in invalidPasswords)
            {
                var body = new InputModelUser() { name = "Guilherme", lastName = "Rocha", email = "gui@gmail.com", password = invalid, cpf = "661.345.778-71" };

                var content = JsonContent.Create<InputModelUser>(body);

                var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

                var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                Assert.True(400 == (int)obj.SelectToken("status"));
                Assert.Equal("The field password is invalid.", (string)obj.SelectToken("errors.password[0]"));
            }
        }

        [Fact, TestPriority(1)]
        public async Task SignUp_User_Invalid_ReturnsUnprocessableRequest()
        {
            var body = new InputModelUser() { name = "Guilherme", lastName = "Rocha", email = "gui@gmail.com", password = "Teste123", cpf = "661.345.778-71" };

            var content = JsonContent.Create<InputModelUser>(body);

            var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

            var obj = await response.Content.ReadFromJsonAsync<OutputModelResponseError>(_options);

            Assert.NotNull(obj);
            Assert.True(obj is OutputModelResponseError);
            Assert.True(422 == obj.Status);
            Assert.Equal("User already exists", obj.Error);
        }

        [Fact, TestPriority(2)]
        public async Task SignUp_User_Invalid_ReturnsBadRequest()
        {

            var body = new InputModelUser() { name = "Gu", lastName = "S", email = "gui.com", password = "Teste1", cpf = "661.345.778-71A" };

            var content = JsonContent.Create<InputModelUser>(body);

            var response = await _testContext.Client.PostAsync("/api/login/signUp", content);

            var obj = JObject.Parse(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(400 == (int)obj.SelectToken("status"));
            Assert.Equal("The field name is invalid.", (string)obj.SelectToken("errors.name[0]"));
            Assert.Equal("The field last name is invalid.", (string)obj.SelectToken("errors.lastName[0]"));
            Assert.Equal("The field cpf is invalid.", (string)obj.SelectToken("errors.cpf[0]"));
            Assert.Equal("The field email is invalid.", (string)obj.SelectToken("errors.email[0]"));
            Assert.Equal("The field password is invalid.", (string)obj.SelectToken("errors.password[0]"));
        }

        [Fact, TestPriority(3)]
        public async Task SignIn_ReturnsOkResponse()
        {
            var body = new InputModelLogin() { email = "gui@gmail.com", password = "Teste123" };

            var content = JsonContent.Create<InputModelLogin>(body);

            var response = await _testContext.Client.PostAsync("/api/login/signIn", content);

            var obj = await response.Content.ReadFromJsonAsync<OutputModelLogin>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(obj);
            Assert.True(obj is OutputModelLogin);
            Assert.True(!string.IsNullOrEmpty(obj.token));
        }
    }

}
