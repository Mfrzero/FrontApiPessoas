using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace KeepSafeFront
{
    public partial class CadastroPessoas : Form
    {
        private const string apiUrl = "https://localhost:5001/api/pessoas";
        private const string bffUrl = "https://localhost:7298/api/pessoas";
        private readonly HttpClient httpClient;
        private readonly HttpClient httpClient2;

        public CadastroPessoas()
        {
            InitializeComponent();
            httpClient = new HttpClient();
            httpClient2 = new HttpClient();
            httpClient.BaseAddress = new Uri(apiUrl);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient2.BaseAddress = new Uri(bffUrl);
            httpClient2.DefaultRequestHeaders.Accept.Clear();
            httpClient2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<string> GetJwtTokenApi()
        {
            string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjEiLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE2OTM0NDU1MjEsImV4cCI6MTY5MzQ0OTEyMSwiaWF0IjoxNjkzNDQ1NTIxfQ.Jr-Ce_7xKPPud8VgCzL9nvjDXUNK2azw257io0AvfZU";
            return jwtToken;
        }
        public async Task<string> GetJwtTokenBff()
        {
            string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjAiLCJuYmYiOjE2OTM0NDU2ODksImV4cCI6MTY5MzQ0NTc0OSwiaWF0IjoxNjkzNDQ1Njg5fQ.behIBsILj9RBD7ofpvxCrvbGNJkdV7ljWKGdDquPbG8";
            return jwtToken;
        }

        public async Task AutorizacaoApi()
        {
            string jwtToken = await GetJwtTokenApi();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
        public async Task AutorizacaoBff()
        {
            string jwtToken = await GetJwtTokenBff();
            httpClient2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        //adiciona
        private async void button1_Click(object sender, EventArgs e)
        {
            await AutorizacaoApi();

            var pessoa = new Pessoa
            {
                Id = Convert.ToInt32(textBox1.Text),
                Nome = textBox2.Text,
                Idade = Convert.ToInt32(textBox3.Text),
                Cidade = textBox4.Text,
                Sexo = Convert.ToInt32(comboBox1.SelectedItem),
            };
            var json = JsonConvert.SerializeObject(pessoa);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                MessageBox.Show("Pessoa registrada com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        //atualiza
        private async void button2_Click(object sender, EventArgs e)
        {
            await AutorizacaoApi();
            try
            {
                var pessoa = new Pessoa
                {
                    Id = Convert.ToInt32(textBox1.Text),
                    Nome = textBox2.Text,
                    Idade = Convert.ToInt32(textBox3.Text),
                    Cidade = textBox4.Text,
                    Sexo = Convert.ToInt32(comboBox1.SelectedItem),
                };
                var json = JsonConvert.SerializeObject(pessoa);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync($"{apiUrl}/{pessoa.Id}", content);
                response.EnsureSuccessStatusCode();
                MessageBox.Show("Pessoa atualizada com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        //deleta
        private async void button3_Click(object sender, EventArgs e)
        {
            await AutorizacaoApi();
            var id = Convert.ToInt32(textBox1.Text);

            try
            {
                var response = await httpClient.DeleteAsync($"{apiUrl}/{id}");
                response.EnsureSuccessStatusCode();
                MessageBox.Show("Person deleted successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();

                // Make an API call
                var peopleResponse = await httpClient2.GetAsync($"{bffUrl}");
                peopleResponse.EnsureSuccessStatusCode();
                var peopleJson = await peopleResponse.Content.ReadAsStringAsync();
                var peopleList = JsonConvert.DeserializeObject<IEnumerable<Pessoa>>(peopleJson);

                // Display the list of people in the ListBox
                listBox1.Items.Clear();
                foreach (var p in peopleList)
                {
                    listBox1.Items.Add($"{p.Id}, {p.Nome}, {p.Idade}, {p.Cidade}, {p.Sexo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var response = await httpClient2.GetAsync($"{bffUrl}/media_idades");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                double result = JsonConvert.DeserializeObject<double>(json);
                MessageBox.Show($"Média de idades: {result}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var response = await httpClient2.GetAsync($"{bffUrl}/total");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var totalPeople = JsonConvert.DeserializeObject<int>(json);
                MessageBox.Show($"Total de pessoas: {totalPeople}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var cidade = textBox6.Text;
                var peopleResponse = await httpClient2.GetAsync($"{bffUrl}/cidade?cidade={cidade}");
                peopleResponse.EnsureSuccessStatusCode();
                var peopleJson = await peopleResponse.Content.ReadAsStringAsync();
                var peopleList = JsonConvert.DeserializeObject<IEnumerable<Pessoa>>(peopleJson);

                // Display the list of people in the ListBox
                listBox1.Items.Clear();
                foreach (var p in peopleList)
                {
                    listBox1.Items.Add($"{p.Id}, {p.Nome}, {p.Idade}, {p.Cidade}, {p.Sexo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button8_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var response = await httpClient2.GetAsync($"{bffUrl}/menores");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var peopleList = JsonConvert.DeserializeObject<IEnumerable<Pessoa>>(json);

                // Display the list of people in the ListBox
                listBox1.Items.Clear();
                foreach (var p in peopleList)
                {
                    listBox1.Items.Add($"{p.Id}, {p.Nome}, {p.Idade}, {p.Cidade}, {p.Sexo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var response = await httpClient2.GetAsync($"{bffUrl}/maiores");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var peopleList = JsonConvert.DeserializeObject<IEnumerable<Pessoa>>(json);

                // Display the list of people in the ListBox
                listBox1.Items.Clear();
                foreach (var p in peopleList)
                {
                    listBox1.Items.Add($"{p.Id}, {p.Nome}, {p.Idade}, {p.Cidade}, {p.Sexo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var nome = textBox5.Text;
                var peopleResponse = await httpClient2.GetAsync($"{bffUrl}/nome?nome={nome}");
                peopleResponse.EnsureSuccessStatusCode();
                var peopleJson = await peopleResponse.Content.ReadAsStringAsync();
                var peopleList = JsonConvert.DeserializeObject<IEnumerable<Pessoa>>(peopleJson);

                // Display the list of people in the ListBox
                listBox1.Items.Clear();
                foreach (var p in peopleList)
                {
                    listBox1.Items.Add($"{p.Id}, {p.Nome}, {p.Idade}, {p.Cidade}, {p.Sexo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var sexo = Convert.ToInt32(comboBox2.SelectedItem);
                var peopleResponse = await httpClient2.GetAsync($"{bffUrl}/sexo?sexo={sexo}");
                peopleResponse.EnsureSuccessStatusCode();
                var peopleJson = await peopleResponse.Content.ReadAsStringAsync();
                var peopleList = JsonConvert.DeserializeObject<IEnumerable<Pessoa>>(peopleJson);

                // Display the list of people in the ListBox
                listBox1.Items.Clear();
                foreach (var p in peopleList)
                {
                    listBox1.Items.Add($"{p.Id}, {p.Nome}, {p.Idade}, {p.Cidade}, {p.Sexo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            try
            {
                await AutorizacaoBff();
                var sexo = Convert.ToInt32(comboBox3.SelectedItem);
                var peopleResponse = await httpClient2.GetAsync($"{bffUrl}/total_sexo?sexo={sexo}");
                peopleResponse.EnsureSuccessStatusCode();
                var peopleJson = await peopleResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<int>(peopleJson);
                if (sexo == 0)
                {
                    MessageBox.Show($"Total por sexo masculino: {result}");
                }
                else
                {
                    MessageBox.Show($"Total por sexo feminino: {result}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
        }

        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
