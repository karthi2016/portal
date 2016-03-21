var membersuite = membersuite || {};
membersuite.paymentProcessor = membersuite.paymentProcessor || {};

membersuite.paymentProcessor = (function ($, ppAPI, msAPI) {
    var
        ppConfig = {
            IsPreferredConfigured: false,
            CustomerID: "0",
            AccessToken: "0",
            EndpointUri: '',
            LoggingUri: ''
        },
        msConfig = {
            guid: '',
            $cardNumberElem: null,
            $expiryMonthElem: null,
            $expiryYearElem: null,
            saveBtnId: null,
            postback: null

        },
        originalCardNum = '',
        originalBankAccNum = '';
        saveLastUnderscore = false;

    function init(parms) {
        ppConfig = parms.ppConfig;
        msConfig = parms.msConfig;
        if (parms.hasOwnProperty('saveLastUnderscore'))
            saveLastUnderscore = parms.saveLastUnderscore;

        if (!!msConfig.$cardNumberElem) {
            ppConfig.CustomerID == null ? createCustomerAndCard() : addCardToCustomer();
        } else if (!!msConfig.$bankAccElement) {
            ppConfig.CustomerID == null ? createCustomerAndBankAccount() : addBankAccToCustomer(); 
        }
    };

    function vaultTokenCreated(response) {
        if (response.isSuccessful) {
            if (!!msConfig.$cardNumberElem) {
                var parms = {
                    vaultToken: response.vaultToken,
                    saveBtnId: msConfig.saveBtnId,
                    postback: msConfig.postback,
                    originalCardNumber: originalCardNum,
                    $cardNumberElem: msConfig.$cardNumberElem,
                    saveLastUnderscore: saveLastUnderscore,
                    logUrl: ppConfig.LoggingUri
                };
                msAPI.persistToken(parms);
            } else if (!!msConfig.$bankAccElement) {
                var parms = {
                    vaultToken: response.vaultToken,
                    saveBtnId: msConfig.saveBtnId,
                    postback: msConfig.postback,
                    originalBankAccNum: originalBankAccNum,
                    $bankAccElement: msConfig.$bankAccElement,
                    $bankAccElementConfirm: msConfig.$bankAccElementConfirm,
                    saveLastUnderscore: saveLastUnderscore,
                    logUrl: ppConfig.LoggingUri
                };
                msAPI.persistToken(parms);
            }
        }
        return response.isSuccessful;
    };

    function addCardToCustomer() {
        var cardNumber = msConfig.$cardNumberElem.val();
        var expiryMonth = msConfig.$expiryMonthElem.val();
        var expiryYear = msConfig.$expiryYearElem.val();
        originalCardNum = cardNumber;

        var payload = {
            "number": cardNumber,
            "expiryMonth": expiryMonth,
            "expiryYear": expiryYear
        };

        var jsonFormData = JSON.stringify(payload);

        var parms = {
            guid: msConfig.guid,
            data: jsonFormData,
            requestToken: ppConfig.AccessToken,
            customerId: ppConfig.CustomerID,
            uri: ppConfig.EndpointUri,
            postback: msConfig.postback,
            originalCardNumber: originalCardNum,
            callback: vaultTokenCreated,
            logUrl: ppConfig.LoggingUri
        }

        //start progress animation
        ppAPI.createVaultToken(parms);
    };

    function createCustomerAndCard(response) {
        var cardNumber = msConfig.$cardNumberElem.val();
        var expiryMonth = msConfig.$expiryMonthElem.val();
        var expiryYear = msConfig.$expiryYearElem.val();
        originalCardNum = cardNumber;





        var payload = {
            "number": msConfig.guid,
            "customerType": ppConfig["CustomerType"],//"person",
            "name": ppConfig["Name"],
            "firstName": ppConfig["FirstName"],
            "lastName": ppConfig["LastName"],
            "email": ppConfig["EmailAddress"],
            "addressName": ppConfig["AddressName"],
            "address1": (ppConfig["Address1"] ? ppConfig["Address1"] : "Line 1"),
            "city": (ppConfig["City"] ? ppConfig["City"] : 'City'),
            "state": (ppConfig["State"] ? ppConfig["State"] : 'No State/Province'),
            "zip": (ppConfig["Zip"] ? ppConfig["Zip"] : '00000'),
            "cardAccount": {
                "number": cardNumber,
                "expiryMonth": expiryMonth,
                "expiryYear": expiryYear,
            }
        }

        var jsonFormData = JSON.stringify(payload);

        var parms = {
            guid: msConfig.guid,
            data: jsonFormData,
            requestToken: ppConfig.AccessToken,
            uri: ppConfig.EndpointUri,
            postback: msConfig.postback,
            originalCardNumber: originalCardNum,
            callback: vaultTokenCreated,
            logUrl: ppConfig.LoggingUri
        }

        //start progress animation
        ppAPI.createCustomerVaultToken(parms);
    };

    function createCustomerAndBankAccount() {
        var bankAccNumber = msConfig.$bankAccElement.val();
        var rtnNumber = msConfig.$rtnNumElement.val();
        var bankAccountType = msConfig.$bankAccTypeElement.val();
        originalBankAccNum = bankAccNumber;

        var payload = {
            "number": msConfig.guid,
            "customerType": ppConfig["CustomerType"],//"person",
            "name": ppConfig["Name"],
            "firstName": ppConfig["FirstName"],
            "lastName": ppConfig["LastName"],
            "email": ppConfig["EmailAddress"],
            "addressName": ppConfig["AddressName"],
            "address1": (ppConfig["Address1"] ? ppConfig["Address1"] : "Line 1"),
            "city": (ppConfig["City"] ? ppConfig["City"] : 'City'),
            "state": (ppConfig["State"] ? ppConfig["State"] : 'No State/Province'),
            "zip": (ppConfig["Zip"] ? ppConfig["Zip"] : '00000'),
            "bankAccount": {
                "name": "BankAccount",
                "alias": "BankAccount",
                "accountNumber": bankAccNumber,
                "routingNumber": rtnNumber,
                "type": bankAccountType
            }
        }

        var jsonFormData = JSON.stringify(payload);

        var parms = {
            guid: msConfig.guid,
            data: jsonFormData,
            requestToken: ppConfig.AccessToken,
            uri: ppConfig.EndpointUri,
            postback: msConfig.postback,
            originalBankAccNumber: originalBankAccNum,
            callback: vaultTokenCreated,
            logUrl: ppConfig.LoggingUri
        }

        ppAPI.createCustomerVaultToken(parms);
    }

    function addBankAccToCustomer() {
        var bankAccNumber = msConfig.$bankAccElement.val();
        var rtnNumber = msConfig.$rtnNumElement.val();
        var bankAccountType = msConfig.$bankAccTypeElement.val();
        originalBankAccNum = bankAccNumber;

        var payload = {
            "name": "BankAccount",
            "alias": "BankAccount",
            "accountNumber": bankAccNumber,
            "routingNumber": rtnNumber,
            "type": bankAccountType,
        };

        var jsonFormData = JSON.stringify(payload);

        var parms = {
            guid: msConfig.guid,
            data: jsonFormData,
            requestToken: ppConfig.AccessToken,
            customerId: ppConfig.CustomerID,
            uri: ppConfig.EndpointUri,
            postback: msConfig.postback,
            originalCardNumber: originalCardNum,
            callback: vaultTokenCreated,
            logUrl: ppConfig.LoggingUri
        }

        //start progress animation
        ppAPI.createVaultTokenBank(parms);

    }

    return {
        init: init
    };
})(jQuery, membersuite.priorityPaymentAjaxAPI, membersuite.ajaxAPI);
