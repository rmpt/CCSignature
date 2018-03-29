# CCSignature
Biblioteca para assinar dados digitalmente a partir de certificados do cartão de cidadão

Classe principal: **CertificateManager**

Métodos a usar para assinatura:
 1. GetStoreCertificates
 2. Sign

O método Sign pressupõe a seleccção de um dos certificatos devolvidos pelo método GetStoreCertificates.

**Exemplo:**

Utilizando a DLL da última release, podemos assinar a string da seguinte forma:

    var certificateManager = new CertificateManager();
    var availableCertificates = certificateManager.GetStoreCertificates();
    var selectedCertificate = availableCertificates[0]; // deve ser seleccionado de alguma forma
    var docToSign = "My document to sign";
    
    var base64Signature = certificateManager.Sign(selectedCertificate.Id, docToSign);
