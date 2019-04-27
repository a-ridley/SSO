using System;
using System.Data.Entity.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccessLayer.Database;
using DataAccessLayer.Models;
using ServiceLayer.Services;
using System.Collections.Generic;

namespace UnitTesting
{
    [TestClass]
    public class SignatureServiceUT
    {
        TestingUtils tu;

        public SignatureServiceUT()
        {
            tu = new TestingUtils();
        }

        // Signatures should match if everything is the same
        [TestMethod]
        public void Sign_Launch_Dict_Success()
        {
            // Setup
            var launchPayload = new Dictionary<string, string>();
            launchPayload.Add("ssoUserId", "example");
            launchPayload.Add("email", "example2");
            launchPayload.Add("timestamp", "example3");
            var secretKey = "supersecretexample";
            var expectedSignatureResult = "bGqhkWq4Nia3jU7TYj/2i2ewG79ZtkLXwLrDy+mFSVc="; // Hardcoded from a previous run

            // Run
            var signatureService = new SignatureService();
            var signature = signatureService.Sign(secretKey, launchPayload);

            // Assert
            Assert.IsNotNull(signature);
            Assert.AreEqual(expectedSignatureResult, signature);
        }

        // Signatures should not match if different keys are used
        [TestMethod]
        public void Sign_Launch_Dict_Different_Keys()
        {
            // Setup
            var launchPayload = new Dictionary<string, string>();
            launchPayload.Add("ssoUserId", "example");
            launchPayload.Add("email", "example2");
            launchPayload.Add("timestamp", "example3");
            var secretKey1 = "supersecretexample";
            var secretKey2 = "supersecretexample2";

            // Run
            var signatureService = new SignatureService();
            var signature1 = signatureService.Sign(secretKey1, launchPayload);
            var signature2 = signatureService.Sign(secretKey2, launchPayload);

            // Assert
            Assert.IsNotNull(signature1);
            Assert.IsNotNull(signature2);
            Assert.AreNotEqual(signature1, signature2);
        }

        // Signatures should not match if data is different
        [TestMethod]
        public void Sign_Launch_Dict_Different_Data()
        {
            // Setup
            var launchPayload = new Dictionary<string, string>();
            launchPayload.Add("ssoUserId", "example");
            launchPayload.Add("email", "example2");
            launchPayload.Add("timestamp", "example3");
            var secretKey = "supersecretexample";

            // Run
            var signatureService = new SignatureService();
            var signature1 = signatureService.Sign(secretKey, launchPayload);

            launchPayload.Add("additionalprop", "example4");
            var signature2 = signatureService.Sign(secretKey, launchPayload);

            // Assert
            Assert.IsNotNull(signature1);
            Assert.IsNotNull(signature2);
            Assert.AreNotEqual(signature1, signature2);
        }

        // Signatures should match if everything is the same
        [TestMethod]
        public void Sign_Launch_String_Success()
        {
            // Setup
            var payloadString = "test string value";
            var secretKey = "supersecretexample";
            var expectedSignatureResult = "CTDn+k8xvw8qkO3ZCoDZ9xI0ZzSjfeZqW5WngBmJbEc="; // Hardcoded from a previous run

            // Run
            var signatureService = new SignatureService();
            var signature = signatureService.Sign(secretKey, payloadString);

            // Assert
            Assert.IsNotNull(signature);
            Assert.AreEqual(signature, expectedSignatureResult);
        }

        // Signatures should not match if different keys are used
        [TestMethod]
        public void Sign_Launch_String_Different_Keys()
        {
            // Setup
            var payloadString = "test string value";
            var secretKey1 = "supersecretexample";
            var secretKey2 = "supersecretexample2";

            // Run
            var signatureService = new SignatureService();
            var signature1 = signatureService.Sign(secretKey1, payloadString);
            var signature2 = signatureService.Sign(secretKey2, payloadString);

            // Assert
            Assert.IsNotNull(signature1);
            Assert.IsNotNull(signature2);
            Assert.AreNotEqual(signature1, signature2);
        }

        // Signatures should not match if data is different
        [TestMethod]
        public void Sign_Launch_String_Different_Data()
        {
            // Setup
            var secretKey = "supersecretexample";
            var signatureService = new SignatureService();

            // Run
            var payloadString1 = "test string value";
            var signature1 = signatureService.Sign(secretKey, payloadString1);

            var payloadString2 = "different string value";
            var signature2 = signatureService.Sign(secretKey, payloadString2);

            // Assert
            Assert.IsNotNull(signature1);
            Assert.IsNotNull(signature2);
            Assert.AreNotEqual(signature1, signature2);
        }
    }
}
