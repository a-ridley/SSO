const puppeteer = require ('puppeteer');
const sso = 'http://localhost:8080/#'
const appTitle = 'My Application';
const validEmail = 'cf2080@gmail.com';
const invalidEmail = 'c@hotmail.com';
const validPassword = 'qazwsx136_!2019';
const invalidPassword = 'dneifskncawisfncewiajknfmciewahnefui!';
const headlessBrowser = {head: false};

async function LoginValid() {
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');

  //Input all fields
  await page.type('#email', validEmail);
  await page.type('#password', validPassword);

  //Click login
  await page.click('#login'); 

  await browser.close()
};

async function LoginInvalidEmail(){
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');

  //Input all fields
  await page.type('#email', invalidEmail);
  await page.type('#password', validPassword);

  //Click login
  await page.click('#login'); 
  await browser.close()
};

async function LoginInvalidPassword(){
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');

  //Input all fields
  await page.type('#email', validEmail);
  await page.type('#password', invalidPassword);

  //Click login
  await page.click('#login'); 

  await browser.close()
};

async function LoginUserDisabled(){
  let imgPath = 'LoginUserDisabled';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');

  //Input all fields
  await page.type('#email', validEmail);
  await page.type('#password', invalidPassword);

  //Click login
  await page.click('#login'); 
  await page.click('#login'); 
  await page.click('#login'); 
  await page.click('#login'); 

  await browser.close()
};

async function LoginBlankFields(){
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  //Click login
  await page.click('#login'); 

  await browser.close()
};

async function LoginBlankEmail(){
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');

  //Input all fields
  await page.type('#password', validPassword);

  //Click login
  await page.click('#login'); 
  await browser.close()
};

async function LoginBlankPassword(){
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');

  //Input all fields
  await page.type('#email', invalidEmail);
  
  //Click login
  await page.click('#login'); 

  await browser.close()
};

async function LoginRedirectResetPassword(){
  // Open browser and navigate to login page
  const browser = await puppeteer.launch(headlessBrowser);
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.click('#reset'); 
  await browser.close()
};

async function Run(){
  LoginValid();
  LoginInvalidEmail();
  LoginInvalidPassword();
  LoginUserDisabled();
  LoginBlankFields();
  LoginBlankEmail();
  LoginBlankPassword();
  LoginRedirectResetPassword();
}

Run();