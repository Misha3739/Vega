using AutoMapper;
using vega.Controllers.Resources;
using vega.Models;
using System.Linq;
using System.Collections.Generic;

namespace vega.Mapping {
    public class MappingProfile : Profile{
        public MappingProfile() {
            // Domain to API resource

            CreateMap<Make, MakeResource>();
            CreateMap<Model, ModelResource>();
            CreateMap<Feature, FeatureResource>();
            CreateMap<Vehicle, VehicleResource>()
            .ForMember(vr => vr.Contact, opt => opt.MapFrom(v => new ContacResource {
                Name = v.ContactName,
                Phone = v.ContactPhone,
                Email = v.ContactEmail
            }))
            .ForMember(vr => vr.Features, opt => opt.MapFrom(v => v.Features.Select(vf => vf.FeatureId)));

            // API resource to Domain

            CreateMap<VehicleResource, Vehicle>()
            .ForMember(v => v.Id, opt => opt.Ignore())
            .ForMember(v => v.ContactName, opt => opt.MapFrom(vr => vr.Contact.Name))
            .ForMember(v => v.ContactEmail, opt => opt.MapFrom(vr => vr.Contact.Email))
            .ForMember(v => v.ContactPhone, opt => opt.MapFrom(vr => vr.Contact.Phone))
            .ForMember(v => v.Features, opt => opt.Ignore())
            .AfterMap((vr, v) => {
                List<VehicleFeature> removedFeatures = new List<VehicleFeature>();
                foreach(var f in v.Features) {
                    if(!vr.Features.Contains(f.FeatureId)) {
                        removedFeatures.Add(f);
                    }
                }
                removedFeatures.ForEach(f=>v.Features.Remove(f));

                foreach(var id in vr.Features) {
                    if(!v.Features.Any(vf => vf.FeatureId == id) ) {
                       v.Features.Add(new VehicleFeature() {
                           FeatureId = id });
                    }
                }
            });
        }
    }
}