const puppeteer = require ('puppeteer');
//const sso = 'https://kfc-sso.com/#';
const sso = 'http://localhost:8080/#'
const appTitle = 'My Application';
const validEmail = 'cf2080@gmail.com';
const invalidEmail = 'c@hotmail.com';
const validPassword = 'qazwsx136_!2019';
const invalidPassword = 'dneifskncawisfncewiajknfmciewahnefui!';


function timeout(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
};

async function LoginValid() {
  let imgPath = 'LoginValid';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Input all fields
  await page.type('#email', validEmail);
  await page.type('#password', validPassword);
  await page.screenshot({path: (imgPath + '02.png')});

  //Click login
  await page.click('#login'); 
  await timeout(500);

  await page.screenshot({path: (imgPath + '03.png')});

  await timeout(3500)
  await page.screenshot({path: (imgPath + '04.png')});

  await browser.close()
};

async function LoginInvalidEmail(){
  let imgPath = 'LoginInvalidEmail';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Input all fields
  await page.type('#email', invalidEmail);
  await page.type('#password', validPassword);
  await page.screenshot({path: (imgPath + '02.png')});

  //Click login
  await page.click('#login'); 
  await timeout(500);

  await page.screenshot({path: (imgPath + '03.png')});

  await timeout(3500)
  await page.screenshot({path: (imgPath + '04.png')});

  await browser.close()
};

async function LoginInvalidPassword(){
  let imgPath = 'LoginInvalidPassword';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Input all fields
  await page.type('#email', validEmail);
  await page.type('#password', invalidPassword);
  await page.screenshot({path: (imgPath + '02.png')});

  //Click login
  await page.click('#login'); 
  await timeout(500);

  await page.screenshot({path: (imgPath + '03.png')});

  await timeout(3500)
  await page.screenshot({path: (imgPath + '04.png')});

  await browser.close()
};

async function LoginUserDisabled(){
  let imgPath = 'LoginUserDisabled';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Input all fields
  await page.type('#email', validEmail);
  await page.type('#password', invalidPassword);
  await page.screenshot({path: (imgPath + '02.png')});

  //Click login
  await page.click('#login'); 
  await timeout(3500);

  await page.click('#login'); 
  await timeout(3500);

  await page.click('#login'); 
  await timeout(3500);

  await page.click('#login'); 

  await timeout(3500)
  await page.screenshot({path: (imgPath + '03.png')});

  await browser.close()
};

async function LoginBlankFields(){
  let imgPath = 'LoginBlankFields';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Click login
  await page.click('#login'); 
  await timeout(500);

  await page.screenshot({path: (imgPath + '02.png')});

  await timeout(3500)
  await page.screenshot({path: (imgPath + '03.png')});

  await browser.close()
};

async function LoginBlankEmail(){
  let imgPath = 'LoginBlankEmail';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Input all fields
  await page.type('#password', validPassword);
  await page.screenshot({path: (imgPath + '02.png')});

  //Click login
  await page.click('#login'); 
  await timeout(500);

  await page.screenshot({path: (imgPath + '03.png')});

  await timeout(3500)
  await page.screenshot({path: (imgPath + '04.png')});

  await browser.close()
};

async function LoginBlankPassword(){
  let imgPath = 'LoginBlankPassword';

  // Open browser and navigate to login page
  const browser = await puppeteer.launch();
  const page = await browser.newPage();
  await page.goto(sso + '/login');
  await page.screenshot({path: (imgPath + '01.png')});

  //Input all fields
  await page.type('#email', invalidEmail);
  await page.screenshot({path: (imgPath + '02.png')});

  //Click login
  await page.click('#login'); 
  await timeout(500);

  await page.screenshot({path: (imgPath + '03.png')});

  await timeout(3500)
  await page.screenshot({path: (imgPath + '04.png')});

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
}

Run();