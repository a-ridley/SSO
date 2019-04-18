// Deps
const puppeteer = require('puppeteer');
const { expect } = require('chai');

// Constants
const baseURL = process.env.BASE_URL || 'http://localhost:8080/#';
const generateKeyUrl = `${baseURL}/key`;
const TYPE_DELAY = 0;
const appTitle = 'My Application';
const appEmail = 'app@email.com';
let randomNum = (start, end) => Math.floor((Math.random() + start) * end);
let randomEmail = () => `${randomNum(1, 99999999)}@${randomNum(1, 99999999)}.com`;
const imgPath = 'AAS_GenerateKey_';

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  };

describe('key generation', () => {
    let browser
    before(async () => {
        browser = await puppeteer.launch({ headless: false });
    })
    after(async () => {
        await browser.close()
    })

    describe('interface fields', () => {
        let page
        before(async () => {
            page = await browser.newPage();
            await page.goto(generateKeyUrl);
            await page.waitForSelector('main h1');
        })
        after(async () => {
            await page.close()
        })

        it('renders expected inputs', async () => {
            expect(await page.$('#title')).to.not.be.null
            expect(await page.$('#email')).to.not.be.null
        })
    })

    describe('invalid entries', () => {
        let page
        before(async () => {
            page = await browser.newPage();
            await page.goto(generateKeyUrl);
            await page.waitForSelector('main h1');
        })
        after(async () => {
            await page.close()
        })

        beforeEach(async () => {
            await page.reload()
            await page.waitForSelector('main h1');
        })

        it('rejects submission with empty values', async () => {
            await page.click('#btnGenerate')
            let error = page.waitForSelector('#error');
            await timeout(1000);
            await page.screenshot({path: (imgPath + 'EmptyValues.png')});
            expect(error).to.not.be.null
        })

        it('rejects submission with invalid email', async () => {
            await page.type('#title', appTitle, {delay: TYPE_DELAY});
            await page.type('#email', 'invalid', {delay: TYPE_DELAY});

            await page.click('#btnGenerate')
            let error = await page.waitForSelector('#error');
            await timeout(1000);
            await page.screenshot({path: (imgPath + 'InvalidEmail.png')});
            expect(error).to.not.be.null
        })

        it('rejects submission with non-existing application', async () => {
            await page.type('#title', appTitle, {delay: TYPE_DELAY});
            await page.type('#email', randomEmail(), {delay: TYPE_DELAY});
            
            await page.click('#btnGenerate')
            let error = await page.waitForSelector('#error');
            await timeout(1000);
            await page.screenshot({path: (imgPath + 'NonExistingApp.png')});
            expect(error).to.not.be.null
        })
    })

    describe('success', () => {
        let page
        before(async () => {
            page = await browser.newPage();
            await page.goto(generateKeyUrl);
            await page.waitForSelector('main h1');
        })
        after(async () => {
            await page.close()
        })

        it('returns a success message', async () => {
            // Input all fields
            await page.type('#title', appTitle, {delay: TYPE_DELAY});
            await page.type('#email', appEmail, {delay: TYPE_DELAY});
            
            await page.click('#btnGenerate');
            let success = await page.waitForSelector('#responseMessage');
            await timeout(1000);
            await page.screenshot({path: (imgPath + 'Success.png')});

            expect(success).to.not.be.null
        })
    })
})
