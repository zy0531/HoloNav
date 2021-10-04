#include "pch-c.h"
#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include "codegen/il2cpp-codegen-metadata.h"





// 0x00000001 System.String VRR.GPSModule.ModuleData::ToString()
extern void ModuleData_ToString_mAC7AD5EABA51AA17849D1FDEA1E4F6E868AE0A01 (void);
// 0x00000002 System.Byte[] VRR.GPSModule.Serializer::GetBytes(System.Byte[],System.Int32,System.Int32,System.Boolean)
extern void Serializer_GetBytes_mE9F918EB3D09F6C4B0E1A1ACDA7307D3478CCCC0 (void);
// 0x00000003 System.Double VRR.GPSModule.Serializer::GetDouble(System.Byte[],System.Int32,System.Boolean)
extern void Serializer_GetDouble_m92BCD61F52842058ACCB14E8F2739CE93ED63147 (void);
// 0x00000004 System.UInt32 VRR.GPSModule.Serializer::GetInt(System.Byte[],System.Int32,System.Boolean)
extern void Serializer_GetInt_mB765F928FC06C84CB1F42649C10A08C8485EB0C8 (void);
// 0x00000005 System.Single VRR.GPSModule.Serializer::GetFloat(System.Byte[],System.Int32,System.Boolean)
extern void Serializer_GetFloat_m2BE8A3890A9B658D6CE0354404898B469B6F6FFE (void);
// 0x00000006 System.Boolean VRR.GPSModule.Serializer::VerifyOutputData(VRR.GPSModule.ModuleData&)
extern void Serializer_VerifyOutputData_mBC6CE06288E2C4DA3254DF24F0F7131DA4BF632B (void);
// 0x00000007 VRR.GPSModule.ModuleData VRR.GPSModule.Serializer::Deserialize(System.Byte[])
extern void Serializer_Deserialize_m5174636CE2E03F279F6E2200FE7797A62FEE8623 (void);
// 0x00000008 System.Void VRR.GPSModule.SerializerException::.ctor(System.String)
extern void SerializerException__ctor_m8D45D91388C5126B93E3D78FC28622A226C46EB3 (void);
static Il2CppMethodPointer s_methodPointers[8] = 
{
	ModuleData_ToString_mAC7AD5EABA51AA17849D1FDEA1E4F6E868AE0A01,
	Serializer_GetBytes_mE9F918EB3D09F6C4B0E1A1ACDA7307D3478CCCC0,
	Serializer_GetDouble_m92BCD61F52842058ACCB14E8F2739CE93ED63147,
	Serializer_GetInt_mB765F928FC06C84CB1F42649C10A08C8485EB0C8,
	Serializer_GetFloat_m2BE8A3890A9B658D6CE0354404898B469B6F6FFE,
	Serializer_VerifyOutputData_mBC6CE06288E2C4DA3254DF24F0F7131DA4BF632B,
	Serializer_Deserialize_m5174636CE2E03F279F6E2200FE7797A62FEE8623,
	SerializerException__ctor_m8D45D91388C5126B93E3D78FC28622A226C46EB3,
};
extern void ModuleData_ToString_mAC7AD5EABA51AA17849D1FDEA1E4F6E868AE0A01_AdjustorThunk (void);
static Il2CppTokenAdjustorThunkPair s_adjustorThunks[1] = 
{
	{ 0x06000001, ModuleData_ToString_mAC7AD5EABA51AA17849D1FDEA1E4F6E868AE0A01_AdjustorThunk },
};
static const int32_t s_InvokerIndices[8] = 
{
	4552,
	5302,
	5480,
	5501,
	5655,
	6535,
	6436,
	3765,
};
extern const CustomAttributesCacheGenerator g_GPSModule_AttributeGenerators[];
IL2CPP_EXTERN_C const Il2CppCodeGenModule g_GPSModule_CodeGenModule;
const Il2CppCodeGenModule g_GPSModule_CodeGenModule = 
{
	"GPSModule.dll",
	8,
	s_methodPointers,
	1,
	s_adjustorThunks,
	s_InvokerIndices,
	0,
	NULL,
	0,
	NULL,
	0,
	NULL,
	NULL,
	g_GPSModule_AttributeGenerators,
	NULL, // module initializer,
	NULL,
	NULL,
	NULL,
};
