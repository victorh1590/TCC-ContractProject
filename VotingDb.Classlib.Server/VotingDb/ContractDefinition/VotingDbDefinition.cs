using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace VotingDb.Classlib.VotingDb.ContractDefinition
{
    public partial class VotingDbDeployment : VotingDbDeploymentBase
    {
        public VotingDbDeployment() : base(BYTECODE) { }
        public VotingDbDeployment(string byteCode) : base(byteCode) { }
    }

    public class VotingDbDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "60806040526040518060400160405280600f81526020017f53656374696f6e4e6f74466f756e640000000000000000000000000000000000815250600090816200004a9190620008f1565b506040518060400160405280601181526020017f43616e6469646174654e6f74466f756e6400000000000000000000000000000081525060019081620000919190620008f1565b506040518060400160405280600d81526020017f436f6e7472616374456d7074790000000000000000000000000000000000000081525060029081620000d89190620008f1565b506040518060400160405280601381526020017f4372656174696f6e44617461496e76616c696400000000000000000000000000815250600390816200011f9190620008f1565b503480156200012d57600080fd5b5060405162002f3638038062002f36833981810160405281019062000153919062000e45565b6200016784848484620003bc60201b60201c565b33600860006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055508060079081620001b99190620008f1565b508160069080519060200190620001d2929190620004d6565b508260059080519060200190620001eb9291906200058a565b5060005b82518160ff161015620003595760005b84518160ff16101562000342576040518060400160405280878460ff168151811062000230576200022f62000f33565b5b60200260200101518360ff168151811062000250576200024f62000f33565b5b602002602001015162ffffff1681526020016001151581525060046000868560ff168151811062000286576200028562000f33565b5b602002602001015162ffffff1662ffffff168152602001908152602001600020868360ff1681518110620002bf57620002be62000f33565b5b6020026020010151604051620002d6919062000fa4565b908152602001604051809103902060008201518160000160006101000a81548162ffffff021916908362ffffff16021790555060208201518160000160036101000a81548160ff0219169083151502179055509050508080620003399062000ff9565b915050620001ff565b508080620003509062000ff9565b915050620001ef565b5060011515433373ffffffffffffffffffffffffffffffffffffffff167f098ccf385b8796837b724a1711c9a86199be82e545561bb7fe612d77357386ec600630604051620003aa929190620015ab565b60405180910390a450505050620016a4565b60008451118015620003cf575060008351115b8015620003dd575060008251115b8015620003eb575081518451145b80156200041757508251846000815181106200040c576200040b62000f33565b5b602002602001015151145b80156200044157506040518060200160405280600081525080519060200120818051906020012014155b801562000489575060405180602001604052806000815250805190602001208360008151811062000477576200047662000f33565b5b60200260200101518051906020012014155b600390620004cf576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401620004c6919062001680565b60405180910390fd5b5050505050565b82805482825590600052602060002090600901600a90048101928215620005775791602002820160005b838211156200054457835183826101000a81548162ffffff021916908362ffffff160217905550926020019260030160208160020104928301926001030262000500565b8015620005755782816101000a81549062ffffff021916905560030160208160020104928301926001030262000544565b505b509050620005869190620005ea565b5090565b828054828255906000526020600020908101928215620005d7579160200282015b82811115620005d6578251829081620005c59190620008f1565b5091602001919060010190620005ab565b5b509050620005e6919062000609565b5090565b5b8082111562000605576000816000905550600101620005eb565b5090565b5b808211156200062d576000818162000623919062000631565b506001016200060a565b5090565b5080546200063f90620006e0565b6000825580601f1062000653575062000674565b601f016020900490600052602060002090810190620006739190620005ea565b5b50565b600081519050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b60006002820490506001821680620006f957607f821691505b6020821081036200070f576200070e620006b1565b5b50919050565b60008190508160005260206000209050919050565b60006020601f8301049050919050565b600082821b905092915050565b600060088302620007797fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff826200073a565b6200078586836200073a565b95508019841693508086168417925050509392505050565b6000819050919050565b6000819050919050565b6000620007d2620007cc620007c6846200079d565b620007a7565b6200079d565b9050919050565b6000819050919050565b620007ee83620007b1565b62000806620007fd82620007d9565b84845462000747565b825550505050565b600090565b6200081d6200080e565b6200082a818484620007e3565b505050565b5b8181101562000852576200084660008262000813565b60018101905062000830565b5050565b601f821115620008a1576200086b8162000715565b62000876846200072a565b8101602085101562000886578190505b6200089e62000895856200072a565b8301826200082f565b50505b505050565b600082821c905092915050565b6000620008c660001984600802620008a6565b1980831691505092915050565b6000620008e18383620008b3565b9150826002028217905092915050565b620008fc8262000677565b67ffffffffffffffff81111562000918576200091762000682565b5b620009248254620006e0565b6200093182828562000856565b600060209050601f83116001811462000969576000841562000954578287015190505b620009608582620008d3565b865550620009d0565b601f198416620009798662000715565b60005b82811015620009a3578489015182556001820191506020850194506020810190506200097c565b86831015620009c35784890151620009bf601f891682620008b3565b8355505b6001600288020188555050505b505050505050565b6000604051905090565b600080fd5b600080fd5b600080fd5b6000601f19601f8301169050919050565b62000a0d82620009f1565b810181811067ffffffffffffffff8211171562000a2f5762000a2e62000682565b5b80604052505050565b600062000a44620009d8565b905062000a52828262000a02565b919050565b600067ffffffffffffffff82111562000a755762000a7462000682565b5b602082029050602081019050919050565b600080fd5b600067ffffffffffffffff82111562000aa95762000aa862000682565b5b602082029050602081019050919050565b600062ffffff82169050919050565b62000ad48162000aba565b811462000ae057600080fd5b50565b60008151905062000af48162000ac9565b92915050565b600062000b1162000b0b8462000a8b565b62000a38565b9050808382526020820190506020840283018581111562000b375762000b3662000a86565b5b835b8181101562000b64578062000b4f888262000ae3565b84526020840193505060208101905062000b39565b5050509392505050565b600082601f83011262000b865762000b85620009ec565b5b815162000b9884826020860162000afa565b91505092915050565b600062000bb862000bb28462000a57565b62000a38565b9050808382526020820190506020840283018581111562000bde5762000bdd62000a86565b5b835b8181101562000c2c57805167ffffffffffffffff81111562000c075762000c06620009ec565b5b80860162000c16898262000b6e565b8552602085019450505060208101905062000be0565b5050509392505050565b600082601f83011262000c4e5762000c4d620009ec565b5b815162000c6084826020860162000ba1565b91505092915050565b600067ffffffffffffffff82111562000c875762000c8662000682565b5b602082029050602081019050919050565b600080fd5b600067ffffffffffffffff82111562000cbb5762000cba62000682565b5b62000cc682620009f1565b9050602081019050919050565b60005b8381101562000cf357808201518184015260208101905062000cd6565b60008484015250505050565b600062000d1662000d108462000c9d565b62000a38565b90508281526020810184848401111562000d355762000d3462000c98565b5b62000d4284828562000cd3565b509392505050565b600082601f83011262000d625762000d61620009ec565b5b815162000d7484826020860162000cff565b91505092915050565b600062000d9462000d8e8462000c69565b62000a38565b9050808382526020820190506020840283018581111562000dba5762000db962000a86565b5b835b8181101562000e0857805167ffffffffffffffff81111562000de35762000de2620009ec565b5b80860162000df2898262000d4a565b8552602085019450505060208101905062000dbc565b5050509392505050565b600082601f83011262000e2a5762000e29620009ec565b5b815162000e3c84826020860162000d7d565b91505092915050565b6000806000806080858703121562000e625762000e61620009e2565b5b600085015167ffffffffffffffff81111562000e835762000e82620009e7565b5b62000e918782880162000c36565b945050602085015167ffffffffffffffff81111562000eb55762000eb4620009e7565b5b62000ec38782880162000e12565b935050604085015167ffffffffffffffff81111562000ee75762000ee6620009e7565b5b62000ef58782880162000b6e565b925050606085015167ffffffffffffffff81111562000f195762000f18620009e7565b5b62000f278782880162000d4a565b91505092959194509250565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b600081905092915050565b600062000f7a8262000677565b62000f86818562000f62565b935062000f9881856020860162000cd3565b80840191505092915050565b600062000fb2828462000f6d565b915081905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b600060ff82169050919050565b6000620010068262000fec565b915060ff82036200101c576200101b62000fbd565b5b600182019050919050565b600081549050919050565b600082825260208201905092915050565b60008190508160005260206000209050919050565b620010638162000aba565b82525050565b60008160001c9050919050565b600062ffffff82169050919050565b60006200109c620010968362001069565b62001076565b9050919050565b60008160181c9050919050565b6000620010c7620010c183620010a3565b62001076565b9050919050565b60008160301c9050919050565b6000620010f2620010ec83620010ce565b62001076565b9050919050565b60008160481c9050919050565b60006200111d6200111783620010f9565b62001076565b9050919050565b60008160601c9050919050565b600062001148620011428362001124565b62001076565b9050919050565b60008160781c9050919050565b6000620011736200116d836200114f565b62001076565b9050919050565b60008160901c9050919050565b60006200119e62001198836200117a565b62001076565b9050919050565b60008160a81c9050919050565b6000620011c9620011c383620011a5565b62001076565b9050919050565b60008160c01c9050919050565b6000620011f4620011ee83620011d0565b62001076565b9050919050565b60008160d81c9050919050565b60006200121f6200121983620011fb565b62001076565b9050919050565b6000620012338262001027565b6200123f818562001032565b9350836200124d8462001043565b600060011562001395575b836001600a0382011015620013945781546200127f88620012798362001085565b62001058565b6020880197506200129b886200129583620010b0565b62001058565b602088019750620012b788620012b183620010db565b62001058565b602088019750620012d388620012cd8362001106565b62001058565b602088019750620012ef88620012e98362001131565b62001058565b6020880197506200130b8862001305836200115c565b62001058565b6020880197506200132788620013218362001187565b62001058565b60208801975062001343886200133d83620011b2565b62001058565b6020880197506200135f886200135983620011dd565b62001058565b6020880197506200137b88620013758362001208565b62001058565b60208801975060018301925050600a8101905062001258565b5b6001156200155957815484821015620013cb57620013be88620013b88362001085565b62001058565b6020880197506001820191505b84821015620013f757620013ea88620013e483620010b0565b62001058565b6020880197506001820191505b84821015620014235762001416886200141083620010db565b62001058565b6020880197506001820191505b848210156200144f5762001442886200143c8362001106565b62001058565b6020880197506001820191505b848210156200147b576200146e88620014688362001131565b62001058565b6020880197506001820191505b84821015620014a7576200149a8862001494836200115c565b62001058565b6020880197506001820191505b84821015620014d357620014c688620014c08362001187565b62001058565b6020880197506001820191505b84821015620014ff57620014f288620014ec83620011b2565b62001058565b6020880197506001820191505b848210156200152b576200151e886200151883620011dd565b62001058565b6020880197506001820191505b8482101562001557576200154a88620015448362001208565b62001058565b6020880197506001820191505b505b8694505050505092915050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000620015938262001566565b9050919050565b620015a58162001586565b82525050565b60006040820190508181036000830152620015c7818562001226565b9050620015d860208301846200159a565b9392505050565b600082825260208201905092915050565b60008154620015ff81620006e0565b6200160b8186620015df565b94506001821660008114620016295760018114620016405762001677565b60ff19831686528115156020028601935062001677565b6200164b8562000715565b60005b838110156200166f578154818901526001820191506020810190506200164e565b808801955050505b50505092915050565b600060208201905081810360008301526200169c8184620015f0565b905092915050565b61188280620016b46000396000f3fe608060405234801561001057600080fd5b50600436106100885760003560e01c8063851b6ef21161005b578063851b6ef21461014e578063a4fe87761461016e578063c19ebc6b1461019f578063db62c438146101bd57610088565b806322d16ad01461008d5780632338cb54146100be5780634aa5adcf146100ee5780636eaf1c911461011e575b600080fd5b6100a760048036038101906100a29190611011565b6101ed565b6040516100b5929190611127565b60405180910390f35b6100d860048036038101906100d3919061118a565b6103b0565b6040516100e591906111c6565b60405180910390f35b61010860048036038101906101039190611011565b610465565b60405161011591906111c6565b60405180910390f35b610138600480360381019061013391906111e1565b610535565b60405161014591906111c6565b60405180910390f35b61015661059d565b604051610165939291906114af565b60405180910390f35b6101886004803603810190610183919061118a565b6108f9565b6040516101969291906114fb565b60405180910390f35b6101a7610af8565b6040516101b491906111c6565b60405180910390f35b6101d760048036038101906101d2919061118a565b610c08565b6040516101e4919061154d565b60405180910390f35b6060806101f983610c1c565b600060068054905067ffffffffffffffff81111561021a57610219610ee6565b5b6040519080825280602002602001820160405280156102485781602001602082028036833780820191505090505b50905060005b6006805490508160ff161015610326576004600060068360ff168154811061027957610278611568565b5b90600052602060002090600a91828204019190066003029054906101000a900462ffffff1662ffffff1662ffffff168152602001908152602001600020856040516102c491906115d3565b908152602001604051809103902060000160009054906101000a900462ffffff16828260ff16815181106102fb576102fa611568565b5b602002602001019062ffffff16908162ffffff1681525050808061031e90611626565b91505061024e565b506006818180548060200260200160405190810160405280929190818152602001828054801561039f57602002820191906000526020600020906000905b82829054906101000a900462ffffff1662ffffff16815260200190600301906020826002010492830192600103820291508084116103645790505b505050505091509250925050915091565b60006103bb82610d03565b6000805b6005805490508160ff16101561045b57600460008562ffffff1662ffffff16815260200190815260200160002060058260ff168154811061040357610402611568565b5b9060005260206000200160405161041a9190611747565b908152602001604051809103902060000160009054906101000a900462ffffff1682610446919061175e565b9150808061045390611626565b9150506103bf565b5080915050919050565b600061047082610c1c565b6000805b6006805490508160ff16101561052b576004600060068360ff168154811061049f5761049e611568565b5b90600052602060002090600a91828204019190066003029054906101000a900462ffffff1662ffffff1662ffffff168152602001908152602001600020846040516104ea91906115d3565b908152602001604051809103902060000160009054906101000a900462ffffff1682610516919061175e565b9150808061052390611626565b915050610474565b5080915050919050565b600061054082610c1c565b61054983610d03565b600460008462ffffff1662ffffff1681526020019081526020016000208260405161057491906115d3565b908152602001604051809103902060000160009054906101000a900462ffffff16905092915050565b60608060606105aa610db0565b600060068054905067ffffffffffffffff8111156105cb576105ca610ee6565b5b6040519080825280602002602001820160405280156105fe57816020015b60608152602001906001900390816105e95790505b50905060005b6006805490508160ff16101561079857600060058054905067ffffffffffffffff81111561063557610634610ee6565b5b6040519080825280602002602001820160405280156106635781602001602082028036833780820191505090505b50905060005b6005805490508160ff161015610761576004600060068560ff168154811061069457610693611568565b5b90600052602060002090600a91828204019190066003029054906101000a900462ffffff1662ffffff1662ffffff16815260200190815260200160002060058260ff16815481106106e8576106e7611568565b5b906000526020600020016040516106ff9190611747565b908152602001604051809103902060000160009054906101000a900462ffffff16828260ff168151811061073657610735611568565b5b602002602001019062ffffff16908162ffffff1681525050808061075990611626565b915050610669565b5080838360ff168151811061077957610778611568565b5b602002602001018190525050808061079090611626565b915050610604565b5060066005828280548060200260200160405190810160405280929190818152602001828054801561081357602002820191906000526020600020906000905b82829054906101000a900462ffffff1662ffffff16815260200190600301906020826002010492830192600103820291508084116107d85790505b5050505050925081805480602002602001604051908101604052809291908181526020016000905b828210156108e757838290600052602060002001805461085a9061167e565b80601f01602080910402602001604051908101604052809291908181526020018280546108869061167e565b80156108d35780601f106108a8576101008083540402835291602001916108d3565b820191906000526020600020905b8154815290600101906020018083116108b657829003601f168201915b50505050508152602001906001019061083b565b50505050915093509350935050909192565b60608061090583610d03565b600060058054905067ffffffffffffffff81111561092657610925610ee6565b5b6040519080825280602002602001820160405280156109545781602001602082028036833780820191505090505b50905060005b6005805490508160ff161015610a1757600460008662ffffff1662ffffff16815260200190815260200160002060058260ff168154811061099e5761099d611568565b5b906000526020600020016040516109b59190611747565b908152602001604051809103902060000160009054906101000a900462ffffff16828260ff16815181106109ec576109eb611568565b5b602002602001019062ffffff16908162ffffff16815250508080610a0f90611626565b91505061095a565b5060058181805480602002602001604051908101604052809291908181526020016000905b82821015610ae8578382906000526020600020018054610a5b9061167e565b80601f0160208091040260200160405190810160405280929190818152602001828054610a879061167e565b8015610ad45780601f10610aa957610100808354040283529160200191610ad4565b820191906000526020600020905b815481529060010190602001808311610ab757829003601f168201915b505050505081526020019060010190610a3c565b5050505091509250925050915091565b6000806000905060005b6006805490508160ff161015610c005760005b6005805490508160ff161015610bec576004600060068460ff1681548110610b4057610b3f611568565b5b90600052602060002090600a91828204019190066003029054906101000a900462ffffff1662ffffff1662ffffff16815260200190815260200160002060058260ff1681548110610b9457610b93611568565b5b90600052602060002001604051610bab9190611747565b908152602001604051809103902060000160009054906101000a900462ffffff1683610bd7919061175e565b92508080610be490611626565b915050610b15565b508080610bf890611626565b915050610b02565b508091505090565b6000610c1382610d03565b60019050919050565b600081604051602001610c2f91906115d3565b60405160208183030381529060405280519060200120905060005b6005805490508160ff161015610cc1578160058260ff1681548110610c7257610c71611568565b5b90600052602060002001604051602001610c8c9190611747565b6040516020818303038152906040528051906020012003610cae575050610d00565b8080610cb990611626565b915050610c4a565b5060016040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610cf7919061182a565b60405180910390fd5b50565b600460008262ffffff1662ffffff1681526020019081526020016000206005600081548110610d3557610d34611568565b5b90600052602060002001604051610d4c9190611747565b908152602001604051809103902060000160039054906101000a900460ff16610dad5760006040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610da4919061182a565b60405180910390fd5b50565b60006006805490501480610dc957506000600580549050145b80610e725750600460006006600081548110610de857610de7611568565b5b90600052602060002090600a91828204019190066003029054906101000a900462ffffff1662ffffff1662ffffff1681526020019081526020016000206005600081548110610e3a57610e39611568565b5b90600052602060002001604051610e519190611747565b908152602001604051809103902060000160039054906101000a900460ff16155b15610eb55760026040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401610eac919061182a565b60405180910390fd5b565b6000604051905090565b600080fd5b600080fd5b600080fd5b600080fd5b6000601f19601f8301169050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b610f1e82610ed5565b810181811067ffffffffffffffff82111715610f3d57610f3c610ee6565b5b80604052505050565b6000610f50610eb7565b9050610f5c8282610f15565b919050565b600067ffffffffffffffff821115610f7c57610f7b610ee6565b5b610f8582610ed5565b9050602081019050919050565b82818337600083830152505050565b6000610fb4610faf84610f61565b610f46565b905082815260208101848484011115610fd057610fcf610ed0565b5b610fdb848285610f92565b509392505050565b600082601f830112610ff857610ff7610ecb565b5b8135611008848260208601610fa1565b91505092915050565b60006020828403121561102757611026610ec1565b5b600082013567ffffffffffffffff81111561104557611044610ec6565b5b61105184828501610fe3565b91505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b600062ffffff82169050919050565b61109e81611086565b82525050565b60006110b08383611095565b60208301905092915050565b6000602082019050919050565b60006110d48261105a565b6110de8185611065565b93506110e983611076565b8060005b8381101561111a57815161110188826110a4565b975061110c836110bc565b9250506001810190506110ed565b5085935050505092915050565b6000604082019050818103600083015261114181856110c9565b9050818103602083015261115581846110c9565b90509392505050565b61116781611086565b811461117257600080fd5b50565b6000813590506111848161115e565b92915050565b6000602082840312156111a05761119f610ec1565b5b60006111ae84828501611175565b91505092915050565b6111c081611086565b82525050565b60006020820190506111db60008301846111b7565b92915050565b600080604083850312156111f8576111f7610ec1565b5b600061120685828601611175565b925050602083013567ffffffffffffffff81111561122757611226610ec6565b5b61123385828601610fe3565b9150509250929050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b600081519050919050565b600082825260208201905092915050565b60005b838110156112a3578082015181840152602081019050611288565b60008484015250505050565b60006112ba82611269565b6112c48185611274565b93506112d4818560208601611285565b6112dd81610ed5565b840191505092915050565b60006112f483836112af565b905092915050565b6000602082019050919050565b60006113148261123d565b61131e8185611248565b93508360208202850161133085611259565b8060005b8581101561136c578484038952815161134d85826112e8565b9450611358836112fc565b925060208a01995050600181019050611334565b50829750879550505050505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b600082825260208201905092915050565b60006113c68261105a565b6113d081856113aa565b93506113db83611076565b8060005b8381101561140c5781516113f388826110a4565b97506113fe836110bc565b9250506001810190506113df565b5085935050505092915050565b600061142583836113bb565b905092915050565b6000602082019050919050565b60006114458261137e565b61144f8185611389565b9350836020820285016114618561139a565b8060005b8581101561149d578484038952815161147e8582611419565b94506114898361142d565b925060208a01995050600181019050611465565b50829750879550505050505092915050565b600060608201905081810360008301526114c981866110c9565b905081810360208301526114dd8185611309565b905081810360408301526114f1818461143a565b9050949350505050565b600060408201905081810360008301526115158185611309565b9050818103602083015261152981846110c9565b90509392505050565b60008115159050919050565b61154781611532565b82525050565b6000602082019050611562600083018461153e565b92915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b600081905092915050565b60006115ad82611269565b6115b78185611597565b93506115c7818560208601611285565b80840191505092915050565b60006115df82846115a2565b915081905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b600060ff82169050919050565b600061163182611619565b915060ff8203611644576116436115ea565b5b600182019050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b6000600282049050600182168061169657607f821691505b6020821081036116a9576116a861164f565b5b50919050565b60008190508160005260206000209050919050565b600081546116d18161167e565b6116db8186611597565b945060018216600081146116f6576001811461170b5761173e565b60ff198316865281151582028601935061173e565b611714856116af565b60005b8381101561173657815481890152600182019150602081019050611717565b838801955050505b50505092915050565b600061175382846116c4565b915081905092915050565b600061176982611086565b915061177483611086565b9250828201905062ffffff81111561178f5761178e6115ea565b5b92915050565b600082825260208201905092915050565b600081546117b38161167e565b6117bd8186611795565b945060018216600081146117d857600181146117ee57611821565b60ff198316865281151560200286019350611821565b6117f7856116af565b60005b83811015611819578154818901526001820191506020810190506117fa565b808801955050505b50505092915050565b6000602082019050818103600083015261184481846117a6565b90509291505056fea2646970667358221220a662a5651c79bae4117faccdf3319671d5c52e9006e8df92f4f74cdb630783bc64736f6c63430008130033";
        public VotingDbDeploymentBase() : base(BYTECODE) { }
        public VotingDbDeploymentBase(string byteCode) : base(byteCode) { }
        [Parameter("uint24[][]", "votes", 1)]
        public virtual List<List<uint>> Votes { get; set; }
        [Parameter("string[]", "candidates", 2)]
        public virtual List<string> Candidates { get; set; }
        [Parameter("uint24[]", "sections", 3)]
        public virtual List<uint> Sections { get; set; }
        [Parameter("string", "timestamp", 4)]
        public virtual string Timestamp { get; set; }
    }

    public partial class FindSectionFunction : FindSectionFunctionBase { }

    [Function("findSection", "bool")]
    public class FindSectionFunctionBase : FunctionMessage
    {
        [Parameter("uint24", "sectionID", 1)]
        public virtual uint SectionID { get; set; }
    }

    public partial class GetAllVotesFunction : GetAllVotesFunctionBase { }

    [Function("getAllVotes", typeof(GetAllVotesOutputDTO))]
    public class GetAllVotesFunctionBase : FunctionMessage
    {

    }

    public partial class GetTotalVotesByCandidateFunction : GetTotalVotesByCandidateFunctionBase { }

    [Function("getTotalVotesByCandidate", "uint24")]
    public class GetTotalVotesByCandidateFunctionBase : FunctionMessage
    {
        [Parameter("string", "candidate", 1)]
        public virtual string Candidate { get; set; }
    }

    public partial class GetTotalVotesBySectionFunction : GetTotalVotesBySectionFunctionBase { }

    [Function("getTotalVotesBySection", "uint24")]
    public class GetTotalVotesBySectionFunctionBase : FunctionMessage
    {
        [Parameter("uint24", "sectionID", 1)]
        public virtual uint SectionID { get; set; }
    }

    public partial class GetTotalVotesInBlockFunction : GetTotalVotesInBlockFunctionBase { }

    [Function("getTotalVotesInBlock", "uint24")]
    public class GetTotalVotesInBlockFunctionBase : FunctionMessage
    {

    }

    public partial class GetVotesByCandidateFunction : GetVotesByCandidateFunctionBase { }

    [Function("getVotesByCandidate", typeof(GetVotesByCandidateOutputDTO))]
    public class GetVotesByCandidateFunctionBase : FunctionMessage
    {
        [Parameter("string", "candidate", 1)]
        public virtual string Candidate { get; set; }
    }

    public partial class GetVotesBySectionFunction : GetVotesBySectionFunctionBase { }

    [Function("getVotesBySection", typeof(GetVotesBySectionOutputDTO))]
    public class GetVotesBySectionFunctionBase : FunctionMessage
    {
        [Parameter("uint24", "sectionID", 1)]
        public virtual uint SectionID { get; set; }
    }

    public partial class GetVotesOnSectionByCandidateFunction : GetVotesOnSectionByCandidateFunctionBase { }

    [Function("getVotesOnSectionByCandidate", "uint24")]
    public class GetVotesOnSectionByCandidateFunctionBase : FunctionMessage
    {
        [Parameter("uint24", "sectionID", 1)]
        public virtual uint SectionID { get; set; }
        [Parameter("string", "candidate", 2)]
        public virtual string Candidate { get; set; }
    }

    public partial class HeartbeatEventDTO : HeartbeatEventDTOBase { }

    [Event("heartbeat")]
    public class HeartbeatEventDTOBase : IEventDTO
    {
        [Parameter("address", "account", 1, true )]
        public virtual string Account { get; set; }
        [Parameter("uint256", "block", 2, true )]
        public virtual BigInteger Block { get; set; }
        [Parameter("bool", "elections", 3, true )]
        public virtual bool Elections { get; set; }
        [Parameter("uint24[]", "sections", 4, false )]
        public virtual List<uint> Sections { get; set; }
        [Parameter("address", "contractAddress", 5, false )]
        public virtual string ContractAddress { get; set; }
    }

    public partial class FindSectionOutputDTO : FindSectionOutputDTOBase { }

    [FunctionOutput]
    public class FindSectionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class GetAllVotesOutputDTO : GetAllVotesOutputDTOBase { }

    [FunctionOutput]
    public class GetAllVotesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint24[]", "", 1)]
        public virtual List<uint> ReturnValue1 { get; set; }
        [Parameter("string[]", "", 2)]
        public virtual List<string> ReturnValue2 { get; set; }
        [Parameter("uint24[][]", "", 3)]
        public virtual List<List<uint>> ReturnValue3 { get; set; }
    }

    public partial class GetTotalVotesByCandidateOutputDTO : GetTotalVotesByCandidateOutputDTOBase { }

    [FunctionOutput]
    public class GetTotalVotesByCandidateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint24", "", 1)]
        public virtual uint ReturnValue1 { get; set; }
    }

    public partial class GetTotalVotesBySectionOutputDTO : GetTotalVotesBySectionOutputDTOBase { }

    [FunctionOutput]
    public class GetTotalVotesBySectionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint24", "", 1)]
        public virtual uint ReturnValue1 { get; set; }
    }

    public partial class GetTotalVotesInBlockOutputDTO : GetTotalVotesInBlockOutputDTOBase { }

    [FunctionOutput]
    public class GetTotalVotesInBlockOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint24", "", 1)]
        public virtual uint ReturnValue1 { get; set; }
    }

    public partial class GetVotesByCandidateOutputDTO : GetVotesByCandidateOutputDTOBase { }

    [FunctionOutput]
    public class GetVotesByCandidateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint24[]", "", 1)]
        public virtual List<uint> ReturnValue1 { get; set; }
        [Parameter("uint24[]", "", 2)]
        public virtual List<uint> ReturnValue2 { get; set; }
    }

    public partial class GetVotesBySectionOutputDTO : GetVotesBySectionOutputDTOBase { }

    [FunctionOutput]
    public class GetVotesBySectionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("string[]", "", 1)]
        public virtual List<string> ReturnValue1 { get; set; }
        [Parameter("uint24[]", "", 2)]
        public virtual List<uint> ReturnValue2 { get; set; }
    }

    public partial class GetVotesOnSectionByCandidateOutputDTO : GetVotesOnSectionByCandidateOutputDTOBase { }

    [FunctionOutput]
    public class GetVotesOnSectionByCandidateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint24", "votes", 1)]
        public virtual uint Votes { get; set; }
    }
}
